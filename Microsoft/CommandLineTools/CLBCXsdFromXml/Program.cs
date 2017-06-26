/*****************************
Project:    CODEiverse - Open Source Tools (OST)
            Command Line Based Codee (CLBC)
            http://www.CODEiverse.com
Created By: EJ Alexandra - 2016
            An Abstract Level, llc
License:    Mozilla Public License 2.0
*****************************/
using CODEiverse.OST.Lib.CmdHelpers;
using CODEiverse.OST.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Derive an Xsd from an Xml file.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) Console.WriteLine("Syntax: CLBCXsdFromXml SomeFile.xml");
            else
            {
                var xml = String.Empty;
                FileInfo inputFI = new FileInfo(args[0]);

                if (!inputFI.Exists)
                {
                    String errorMessage = String.Format("Input file: {0} could not be found.", inputFI.FullName);
                    throw new Exception(errorMessage);
                }
                else
                {
                    using (var fileStream = new FileStream(inputFI.FullName, FileMode.Open))
                    {
                        StreamReader sr = new StreamReader(fileStream);
                        fileStream.Position = 0;
                        xml = sr.ReadToEnd();
                    }


                    XDocument srcDoc = XDocument.Parse(xml);

                    XmlReader reader = XmlReader.Create(new StringReader(xml));
                    XmlSchemaInference inference = new XmlSchemaInference();
                    XmlSchemaSet schemaSet = inference.InferSchema(reader);

                    // Display the inferred schema.
                    var index = 1;
                    foreach (XmlSchema schema in schemaSet.Schemas())
                    {
                        MemoryStream sms = new MemoryStream();
                        StreamWriter writer = new StreamWriter(sms);
                        schema.Write(writer);
                        String xsd = Encoding.Default.GetString(sms.GetBuffer(), 0, (int)sms.Length);

                        String inputFileName = inputFI.Name;
                        if (String.Equals(Path.GetExtension(inputFileName), ".xsd", StringComparison.OrdinalIgnoreCase))
                        {
                            inputFileName = inputFileName.Substring(0, inputFileName.Length - 4);
                        }

                        var extension = ".xsd";
                        if (index > 1) extension = index.ToString() + extension;
                        index++;

                        var dsFileName = String.Format("{0}{1}", inputFileName, extension);
                        dsFileName = Path.Combine(inputFI.DirectoryName, dsFileName);
                        File.WriteAllText(dsFileName, xsd);

                        Console.WriteLine(String.Format("Wrote {0} characters to: {1} ", xsd.Length, dsFileName));

                    }
                }
            }
        }
    }
}
