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
            if (args.Length == 2) Console.WriteLine("v1.1");
            FileInfo inputFileInfo = args.GetFirst<FileInfo>();
            XmlDocument doc = new XmlDocument();
            doc.Load(inputFileInfo.FullName);
            String json = doc.XmlToJson();
            Console.Write(json);
        }
    }
}
