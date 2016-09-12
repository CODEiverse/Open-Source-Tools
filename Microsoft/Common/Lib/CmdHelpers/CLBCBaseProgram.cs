using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace CODEiverse.OST.Lib.CmdHelpers
{
    public class CLBCBaseProgram
    {
        /// <summary>
        /// Print the syntax for the current command line tool
        /// </summary>
        /// <param name="syntax">The syntax</param>
        public static void PrintSyntax(String syntax)
        {
            String fullSyntax = String.Format("Sytax: {0} {1}", "cmd.exe", syntax);
            Console.WriteLine(fullSyntax);
        }

        public static void SplitContents(string newFileContents, bool overwriteAll, String basePath)
        {
            if (String.IsNullOrEmpty(newFileContents) || !newFileContents.Contains("<")) return;

            newFileContents = newFileContents.Substring(newFileContents.IndexOf("<"));
            newFileContents = newFileContents.Replace("FileContents><?xml ", "FileContents>&lt;?xml");
            XmlDocument doc = new XmlDocument();
            //doc.PreserveWhitespace = true;
            try
            {
                //if (!newFileContents.StartsWith("<?xml ")) newFileContents = "<?xml version=\"1.0\"?>" + newFileContents;
                int len = newFileContents.Length;
                newFileContents = newFileContents.Trim((char)(65279));
                int len2 = newFileContents.Length;
                doc.LoadXml(newFileContents);
                if (doc.DocumentElement.Name == "FileSet")
                {
                    foreach (XmlElement elem in doc.DocumentElement.SelectNodes("//FileSetFile"))
                    {
                        XmlNode contentsNode = elem.SelectSingleNode("FileContents");
                        XmlNode binaryContentsNode = elem.SelectSingleNode("BinaryFileContents");
                        XmlNode zippedBinaryContentsNode = elem.SelectSingleNode("ZippedBinaryFileContents");
                        XmlNode zippedTextContentsNode = elem.SelectSingleNode("ZippedTextFileContents");

                        if (!ReferenceEquals(contentsNode, null))
                        {
                            String contents = contentsNode.InnerXml;
                            foreach (XmlElement fileName in elem.SelectNodes("RelativePath"))
                            {
                                ProcessFileSetFile(String.Format("{0}\\test.xml", basePath), overwriteAll, elem, contents, fileName, basePath);
                            }
                        }
                        else if (!ReferenceEquals(binaryContentsNode, null) ||
                                !ReferenceEquals(zippedBinaryContentsNode, null) ||
                                !ReferenceEquals(zippedTextContentsNode, null))
                        {

                            String contents = String.Empty;
                            if (!ReferenceEquals(binaryContentsNode, null)) contents = binaryContentsNode.InnerXml;
                            else if (!ReferenceEquals(zippedBinaryContentsNode, null)) contents = zippedBinaryContentsNode.InnerXml;
                            else contents = zippedTextContentsNode.InnerXml;

                            foreach (XmlElement fileName in elem.SelectNodes("RelativePath"))
                            {
                                byte[] data = Convert.FromBase64String(contents);

                                String dir = Path.GetDirectoryName(fileName.InnerText);
                                if (!String.IsNullOrEmpty(dir))
                                {
                                    DirectoryInfo di = new DirectoryInfo(dir);
                                    if (!di.Exists) di.Create();
                                }


                                if (!ReferenceEquals(zippedBinaryContentsNode, null))
                                {
                                    File.WriteAllBytes(fileName.InnerText, data);
                                }
                                else if (!ReferenceEquals(zippedTextContentsNode, null))
                                {
                                    File.WriteAllText(fileName.InnerText, data.Unzip());
                                }
                                else
                                {
                                    File.WriteAllBytes(fileName.InnerText, data);

                                }

                            }
                        }
                        else
                        {
                            foreach (XmlElement fileName in elem.SelectNodes("RelativePath"))
                            {
                                File.WriteAllText(fileName.InnerText, "Can't write binary contents.");
                            }

                        }
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                // Ignore failures attempting to write extra files
                throw ex;
            }
        }

        private static void ProcessFileSetFile(String relativePathOfXml, bool overwriteAll, XmlElement elem, String contents, XmlElement fileName, String basePath)
        {
            String relativeFileName = fileName.InnerText;
            relativeFileName = FullFromRelative(relativePathOfXml, relativeFileName.Trim("\\/ \r\n".ToCharArray()));
            //lastOutputFiles.Add(fullFileName);
            String dir = Path.GetDirectoryName(relativeFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            bool fileExists = File.Exists(relativeFileName);
            bool writeFile = true;
            XmlNode ooNode = elem.SelectSingleNode("OverwriteMode");
            if (fileExists && (!ReferenceEquals(ooNode, null) && (ooNode.InnerText == "Never")))
            {
                writeFile = false;
                //lastOutputFilesSkipped.Add(fullFileName);
            }

            while (contents.Contains("[$$NEWUUID$$]"))
            {
                contents = String.Format("{0}{1}{2}",
                                    contents.Substring(0, contents.IndexOf("[$$NEWUUID$$]")),
                                    Guid.NewGuid(),
                                    contents.Substring(contents.IndexOf("[$$NEWUUID$$]") + "[$$NEWUUID$$]".Length));
            }
            String decodedContent = HttpUtility.HtmlDecode(contents);

            if (overwriteAll || writeFile)
            {
                if (overwriteAll ||
                    !File.Exists(relativeFileName) ||
                    (File.ReadAllText(relativeFileName) != decodedContent))
                {
                    File.WriteAllText(relativeFileName, decodedContent);
                }
            }
        }

        public static void SplitFileSet(String fileName, String basePath)
        {
            String fileContents = File.ReadAllText(fileName);
            SplitContents(fileContents, false, basePath);
        }

        public static string FullFromRelative(string fileName, string relativeFileName)
        {
            relativeFileName = relativeFileName.SafeToString().Trim("\r\n\t \\/".ToCharArray());
            FileInfo fi = new FileInfo(Path.Combine(Path.GetDirectoryName(fileName), relativeFileName));
            return fi.FullName;
        }


        public static string RelativeFromFull(String fileName, String fullPath)
        {
            if (String.IsNullOrEmpty(fullPath)) return fullPath;
            // Trim the path until it doesn't match
            String dir = Path.GetDirectoryName(fileName).Replace("/", "\\");
            String[] dirParts = dir.Split("\\".ToCharArray());
            String[] fullPathParts = fullPath.Replace("/", "\\").Split("\\".ToCharArray());
            int index = 0;
            while (index < fullPathParts.Length && index < dirParts.Length)
            {
                if (String.Equals(fullPathParts[index], dirParts[index], StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
                else
                {
                    break;
                }
            }
            String left = String.Join("\\", dirParts.Take(index));
            String[] parts = left.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            String right = String.Join("\\", fullPathParts.Skip(index));
            int count = dirParts.Length - index;
            for (int i = 0; i < count; i++)
            {
                right = "../" + right;
            }
            return right.Replace("/", "\\");
        }



    }
}
