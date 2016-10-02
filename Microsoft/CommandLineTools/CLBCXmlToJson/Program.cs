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
using System.Xml;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Convert an Xml file into a Json file.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();


            if (!args.Any()) Console.Write("Sytnax: CLBCXmlToJson FileName.xml");
            else
            {
                FileInfo fi = args.GetFirst<FileInfo>();
                if (!fi.Exists) throw new Exception(String.Format("File {0} does not exist.", fi.FullName));

                doc.Load(fi.FullName);
                String json = doc.XmlToJson();

                String jsonFileName = String.Format("{0}.json", fi.FullName);
                File.WriteAllText(jsonFileName, json);
                Console.WriteLine("File generated.");
            }
        }
    }
}
