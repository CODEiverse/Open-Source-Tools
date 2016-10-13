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

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Convert a Json file to an Xml file.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            if (!args.Any()) Console.Write("Sytnax: CLBCJsonToXml FileName.json");
            else
            {
                FileInfo fi = args.GetFirst<FileInfo>();
                if (!fi.Exists) throw new Exception(String.Format("File {0} does not exist.", fi.FullName));

                String json = File.ReadAllText(fi.FullName);
                String xml = json.JsonToXml().OuterXml;

                String xmlFileName = String.Format("{0}.xml", fi.FullName);
                File.WriteAllText(xmlFileName, xml);
                Console.WriteLine("Xml File generated.");
            }
        }
    }
}
