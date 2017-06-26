using Mvp.Xml.Exslt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace CODEiverse.OST.Lib.XsltHandling
{
    public class XsltHandler
    {
        private string Xslt;

        public string Xml { get; set; }
        public string FileSetXml { get; private set; }

        public XsltHandler()
        {
        }

        public void LoadXmlFromFileInfo(FileInfo inputFI)
        {
            // Find the first argument
            if (!inputFI.Exists) throw new Exception(String.Format("Can't load XML File: '{0}'", inputFI.FullName));
            else this.Xml = File.ReadAllText(inputFI.FullName);
        }

        public void Execute()
        {
            // Process the xml - and then pass the output to a split file contents...
            this.FileSetXml = String.Empty;

        }

        public void LoadXslt(string xslt)
        {
            this.Xslt = xslt;
        }


        private void SplitFileSet()
        {
            this.FileSetXml.SplitFileSetFile(Environment.CurrentDirectory);
        }

        private void CookDish(bool clean, String rootPath)
        {
            if (clean) this.Clean();
            this.Cook(rootPath);
        }

        private void Clean()
        {
            // No cleaning to start with
        }

        public void Cook(String rootPath)
        {
            // Processs the xslt file
            this.FileSetXml = this.ProcessXslt(rootPath);
            this.FileSetXml.SplitFileSetXml(false, rootPath);
        }

        private string ProcessXslt(String rootPath)
        {
            var xslt = this.Xslt;
            var xml = this.Xml;
            if (String.IsNullOrEmpty(this.Xslt)) throw new Exception("Must load xslt before calling Execute()");
            else if (String.IsNullOrEmpty(this.Xml)) throw new Exception("No xml loaded for xslt to process");
            else
            {
                String fileSet = String.Empty;

                Environment.CurrentDirectory = rootPath;

                this.Clean();


                ExsltTransform t = PrepareTransformationObject(rootPath, this.Xslt);

                XsltArgumentList al = new XsltArgumentList();
                al.AddParam("dish-root", String.Empty, rootPath);
                al.AddParam("codee-root", String.Empty, rootPath);
                al.AddParam("xml-root", String.Empty, rootPath);
                al.AddParam("xslt-root", String.Empty, rootPath);

                String inputXml = this.Xml;

                String newFileContents = String.Empty;


                XmlDocument xsdDoc = new XmlDocument();
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(inputXml));
                ms.Position = 0;
                XmlReader reader = new XmlTextReader(ms);
                XmlSchemaInference schema = new XmlSchemaInference();
                XmlSchemaSet schemaSet = schema.InferSchema(reader);
                ms = new MemoryStream();
                schemaSet.Schemas().OfType<XmlSchema>().First().Write(ms);
                String xsd = Encoding.Default.GetString(ms.GetBuffer());
                xsdDoc.LoadXml(xsd);
                ms = new MemoryStream();
                try
                {
                    t.Transform(xsdDoc.CreateNavigator(), al, ms);

                    ms.Position = 0;

                    var doc = new XmlDocument();
                    String currentDoc = (new UTF8Encoding(true)).GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    if (currentDoc.StartsWith("<?xml")) currentDoc = currentDoc.Substring(currentDoc.IndexOf("?>") + 2);
                    currentDoc = currentDoc.Trim((char)65279);
                    if (currentDoc.Contains("<"))
                    {
                        currentDoc = currentDoc.Substring(currentDoc.IndexOf("<"));
                    }
                    if (!String.IsNullOrEmpty(currentDoc))
                    {
                        try
                        {
                            doc.LoadXml(currentDoc);
                            MemoryStream wms = new MemoryStream();
                            XmlTextWriter writer = new XmlTextWriter(wms, (new UTF8Encoding(true)));
                            writer.Formatting = Formatting.Indented;
                            doc.WriteContentTo(writer);
                            writer.Flush();
                            newFileContents = (new UTF8Encoding(true)).GetString(wms.GetBuffer(), 0, (int)writer.BaseStream.Length).Trim();
                            wms.Close();
                        }
                        catch (Exception ex)
                        {
                            throw ex;  // Shoudl really call kitchen service below
                            //KitchenService.ReportError(ex);
                            // Ignore formatting errors
                            newFileContents = currentDoc;
                        }
                    }
                }
                finally
                {
                    ms.Close();
                }


                throw new NotImplementedException("The section below still needs to be 'ported'");
                /* 
                String outputFileName = String.Format("{0}{1}", this.FileName, ".Output.xml");

                newFileContents = newFileContents.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine).Trim();

                this.PreviousFileSetZipped = newFileContents.Zip();

                this.Save();

                //File.WriteAllText(outputFileName, newFileContents);
                Dish.SplitContents(outputFileName, newFileContents, false, this.FileName);



                return fileSet;
                */
            }
        }

        public ExsltTransform PrepareTransformationObject(String rootPath, string xsltText)
        {
            ExsltTransform xslt = new ExsltTransform();
            MemoryStream ms = new MemoryStream((new UTF8Encoding(true)).GetBytes(xsltText));
            try
            {
                XmlTextReader xmltr = new XmlTextReader(ms);
                xmltr.WhitespaceHandling = WhitespaceHandling.Significant;
                xslt.Load(xmltr, new MyResolver(rootPath, this));
            }
            finally
            {
                ms.Close();
            }
            return xslt;
        }

        private class MyResolver : XmlResolver
        {
            public MyResolver(String rootPath, XsltHandler handler)
            {
                this.ForDish = handler;
                this.RootPath = rootPath;
                //Console.WriteLine("REsolver for: " + rootPath);
            }

            public override System.Net.ICredentials Credentials
            {
                set { throw new NotImplementedException(); }
            }

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                return new FileStream(this.RootPath.FindClosest(absoluteUri.LocalPath), FileMode.Open);
            }

            public string RootPath { get; set; }

            public XsltHandler ForDish { get; set; }
        }
    }

    /*
    private static void CreateEmptyDish()
    {
        XsltDish xd = new XsltDish();
        DirectoryInfo di = new DirectoryInfo(XsltHandler.Args[0]);

        String fileName = String.Format("{0}.ccxd", di.Name);
        xd.FileName = Path.Combine(di.FullName, fileName);
        Console.Write(xd.FileName);
        xd.Save();
        Process.Start(xd.FileName);
        Console.ReadKey();
    }



    private static void CreateODXMLDish()
    {
        Console.WriteLine("Creating XSLT Dish");
        // Create Find the Nearest ODXML file
        String odxmlPath = Dish.FindClosest(XsltHandler.Args[0], "ODXML\\DataSchema.odxml");
        if (File.Exists(odxmlPath))
        {
            String folderName = Path.GetFileName(XsltHandler.Args[0]);
            String xsltDishFileName = Path.Combine(XsltHandler.Args[0], String.Format("{0}.ccxd", folderName));
            Console.WriteLine("New Dish Path: " + xsltDishFileName);
            XsltDish d = new XsltDish();
            d.FileName = xsltDishFileName;
            d.XmlRelativeFileName = d.GetRelativeFileName(odxmlPath);
            d.CreateDefaultXslt();
            d.Save();
            Process.Start(xsltDishFileName);
        }
        else throw new Exception("Can't find 'nearest' ODXML file.'");
    }
    */

}
