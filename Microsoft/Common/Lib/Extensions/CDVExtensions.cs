/*****************************
Project:    CODEiverse - Open Source Tools (OST)
Created By: EJ Alexandra - 2016
            An Abstract Level, llc
License:    Mozilla Public License 2.0
*****************************/
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace CODEiverse.OST.Lib
{
    public static class CDVExtensions
    {

        public static String FormatXml(this String Xml)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(Xml);

                writer.Formatting = System.Xml.Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException ex)
            {
                throw ex;
            }

            mStream.Close();
            writer.Close();

            return Result;
        }

        public static string FindClosest(this string fullFileName, string otherFile)
        {
            String dir = Path.GetDirectoryName(fullFileName);
            String combinedPath = Path.Combine(dir, otherFile);
            Uri absoluteUri = new Uri(String.Format("file:///{0}", combinedPath));
            //Console.WriteLine(String.Format("resolving: {0}", absoluteUri.ToString()));
            String fileName = absoluteUri.LocalPath;
            String relativeFileName = fullFileName.RelativeFromFull(fileName);
            bool lookDown = true;
            int count = 0;
            while (!File.Exists(fileName) && (count < 10))
            {
                if (relativeFileName.StartsWith("../") && lookDown) fileName = fullFileName.FullFromRelative(relativeFileName.Substring(3));
                else
                {
                    lookDown = false;
                    fileName = fullFileName.FullFromRelative("../" + relativeFileName);
                }

                relativeFileName = fullFileName.RelativeFromFull(fileName);
                count++;
            }

            if (!File.Exists(fileName)) fileName = absoluteUri.LocalPath;

            return fileName;
        }

        public static event EventHandler FileWritten;
        private static void OnFileWritten(String fileName)
        {
            if (!ReferenceEquals(FileWritten, null)) FileWritten(fileName, EventArgs.Empty);
        }

        private static void WriteAllBytes(string fileName, byte[] fileBytes)
        {
            File.WriteAllBytes(fileName, fileBytes);
            OnFileWritten(fileName);
        }

        public static String UnwrapCDATA(this String cdataText)
        {
            cdataText = cdataText.SafeToString();
            if (cdataText.StartsWith("<![CDATA[") && cdataText.EndsWith("]]>"))
            {
                var startPosition = "<![CDATA[".Length;
                var lengthToExclude = "<![CDATA[]]>".Length;
                cdataText = cdataText.Substring(startPosition, cdataText.Length - lengthToExclude);
            }
            return cdataText;
        }

        private static void WriteAllText(string fileName, string fileContents)
        {
            File.WriteAllText(fileName, fileContents.UnwrapCDATA());
            OnFileWritten(fileName);
        }

        public static void SplitFileSetXml(this string fileSetXml, bool overwriteAll, String basePath)
        {
            if (String.IsNullOrEmpty(fileSetXml) || !fileSetXml.Contains("<")) return;

            fileSetXml = fileSetXml.Substring(fileSetXml.IndexOf("<"));
            fileSetXml = fileSetXml.Replace("FileContents><?xml ", "FileContents>&lt;?xml");
            XmlDocument doc = new XmlDocument();
            //doc.PreserveWhitespace = true;
            try
            {
                //if (!newFileContents.StartsWith("<?xml ")) newFileContents = "<?xml version=\"1.0\"?>" + newFileContents;
                int len = fileSetXml.Length;
                fileSetXml = fileSetXml.Trim((char)(65279));
                int len2 = fileSetXml.Length;
                doc.LoadXml(fileSetXml);
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


                                var fileInfo = new FileInfo(fileName.InnerText);

                                if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

                                if (!ReferenceEquals(zippedBinaryContentsNode, null))
                                {
                                    using (StreamWriter sw = new StreamWriter(File.Open(fileName.InnerText, FileMode.Create), Encoding.UTF8))
                                    {
                                        sw.Write(data);
                                    }
                                }
                                else if (!ReferenceEquals(zippedTextContentsNode, null))
                                {

                                    using (StreamWriter sw = new StreamWriter(File.Open(fileName.InnerText, FileMode.Create), Encoding.UTF8))
                                    {
                                        sw.WriteLine(data.Unzip());
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(File.Open(fileName.InnerText, FileMode.Create), Encoding.UTF8))
                                    {
                                        sw.Write(data);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (XmlElement fileName in elem.SelectNodes("RelativePath"))
                            {
                                WriteAllText(fileName.InnerText, "Can't write binary contents.");
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
            decodedContent = decodedContent.UnwrapCDATA();

            if (overwriteAll || writeFile)
            {
                if (overwriteAll ||
                    !File.Exists(relativeFileName) ||
                    (File.ReadAllText(relativeFileName) != decodedContent))
                {
                    WriteAllText(relativeFileName, decodedContent);
                }
            }
        }

        public static void SplitFileSetFile(this String fileSetFileName, String basePath)
        {
            String fileContents = File.ReadAllText(fileSetFileName);
            SplitFileSetXml(fileContents, false, basePath);
        }

        public static string FullFromRelative(this string rootFullPath, string relativeFileName)
        {
            relativeFileName = relativeFileName.SafeToString().Trim("\r\n\t \\/".ToCharArray());
            FileInfo fi = new FileInfo(Path.Combine(Path.GetDirectoryName(rootFullPath), relativeFileName));
            return fi.FullName;
        }

        public static string RelativeFromFull(this String rootFullPath, String fullPath)
        {
            if (String.IsNullOrEmpty(fullPath)) return fullPath;
            // Trim the path until it doesn't match
            String dir = Path.GetDirectoryName(rootFullPath).Replace("/", "\\");
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

        public static string GetTextResourceContents(this Assembly assembly, String resourceName)
        {
            var fullResourceName = assembly.GetManifestResourceNames().FirstOrDefault(fod => fod.Contains(resourceName));
            if (String.IsNullOrEmpty(fullResourceName)) throw new Exception(String.Format("Can't find resource '{0}' in '{1}'", resourceName, assembly.FullName));
            else
            {
                var resourceStream = assembly.GetManifestResourceStream(fullResourceName);
                using (var streamReader = new StreamReader(resourceStream))
                {
                    resourceStream.Position = 0;
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static string SafeToString(this object objectToMakeText)
        {
            if (ReferenceEquals(objectToMakeText, null)) return String.Empty;
            return objectToMakeText.ToString();
        }

        public static String HtmlToXml(this String htmlText)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            MemoryStream ms = new MemoryStream();
            doc.OptionOutputAsXml = true;
            var anchors = doc.DocumentNode
                            .SelectNodes("//a");

            if (!ReferenceEquals(anchors, null))
            {
                anchors.Where(whereNode => !ReferenceEquals(whereNode, null))
                        .ToList()
                        .ForEach(feAnchor => feAnchor.CleanSPECDOCLink());
            }

            doc.Save(ms);
            ms.Position = 0;
            var sr = (new StreamReader(ms));
            return sr.ReadToEnd();
        }

        public static String XmlToJson(this XmlDocument doc)
        {
            return JsonConvert.SerializeXmlNode(doc.DocumentElement, Newtonsoft.Json.Formatting.Indented);

        }

        public static XmlDocument JsonToXml(this String json)
        {
            try
            {
                return JsonConvert.DeserializeXmlNode(json);
            }
            catch (JsonSerializationException jse)
            {
                json = String.Format("{{ root: {0} }}", json);
                return JsonConvert.DeserializeXmlNode(json);
            }

        }

        private static void CleanSPECDOCLink(this HtmlNode node)
        {
            if (!ReferenceEquals(node.Attributes, null))
            {
                var href = node.Attributes["href"];
                if (!ReferenceEquals(href, null))
                {
                    if (!String.IsNullOrEmpty(href.Value))
                    {
                        if (href.Value.Contains("://specdocs/"))
                        {
                            href.Value = href.Value.Substring(href.Value.IndexOf("://specdocs/") + "://specdocs/".Length);
                            var entityIndex = href.Value.ToLower().IndexOf("/entity/");
                            var viewResourceIndex = href.Value.ToLower().IndexOf("/view-resource/");
                            var simpleLinkIndex = href.Value.ToLower().IndexOf("/link/");
                            if (entityIndex >= 0)
                            {
                                var entityName = href.Value.Substring(entityIndex + "/entity/".Length);
                                var candidates = entityName.Split("/&?".ToCharArray());
                                var finalName = candidates.FirstOrDefault(fodCandidate => !String.IsNullOrEmpty(fodCandidate.SafeToString().Trim()));

                                href.Value = String.Format("$SDK_ROOT_URI$Docs/DataModel/Entity_{0}.html", finalName);
                            }
                            else if (viewResourceIndex >= 0)
                            {
                                var resourceName = href.Value.Substring(viewResourceIndex + "/view-resource/".Length);
                                var candidates = resourceName.Split("/&?".ToCharArray());
                                var finalName = candidates.FirstOrDefault(fodCandidate => !String.IsNullOrEmpty(fodCandidate.SafeToString().Trim()));

                                href.Value = String.Format("$SDK_ROOT_URI$Docs/AdditionalResources/Resource_{0}.html", finalName);
                            }
                            else if (simpleLinkIndex >= 0)
                            {
                                href.Value = HttpUtility.UrlDecode(href.Value.Substring(viewResourceIndex + "/link/".Length));
                            }
                            else
                            {
                                href.Value = "ERROR - COULD NOT FIND A MATCHING SPECDOCS// syntax for: '" + href.Value + "'";
                            }

                            if (href.Value.Contains(".html&")) href.Value = href.Value.Substring(0, href.Value.IndexOf(".html&") + ".html".Length);
                        }
                        else if (href.Value.StartsWith("https://www.google.com/url?q="))
                        {
                            var decodedUrl = HttpUtility.UrlDecode(href.Value.Substring("https://www.google.com/url?q=".Length)) + "&amp;";
                            href.Value = decodedUrl.Substring(0, decodedUrl.IndexOf("&amp;"));
                            node.SetAttributeValue("target", "_blank");
                        }
                        else
                        {
                            node.SetAttributeValue("target", "_blank");
                            object o = 1;
                        }
                    }
                }
            }
        }

        public static T GetFirst<T>(this string[] args)
            where T : class
        {
            if (typeof(T) == typeof(String))
            {
                if (ReferenceEquals(args, null)) return String.Empty as T;
                else if (!args.Any()) return String.Empty as T;
                else return args[0] as T;
            }
            else if (typeof(T) == typeof(Uri))
            {
                var uriString = args.GetFirst<String>();
                if (!String.IsNullOrEmpty(uriString))
                {
                    return new Uri(uriString) as T;
                }
            }
            else if (typeof(T) == typeof(FileInfo))
            {
                var fileNameString = args.GetFirst<String>();
                if (!String.IsNullOrEmpty(fileNameString))
                {
                    return new FileInfo(fileNameString) as T;
                }
            }
            else
            {
                var msg = String.Format("Handler not writtent to handle finding first argument of type '{0}'", typeof(T).Name);
                throw new NotImplementedException(msg);
            }

            // If we get here - return nothing
            return default(T);
        }


        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(this byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

    }
}
