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
using System.Diagnostics;
using System.IO;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Interpret html (liberally) to create a valid xhtml document.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            if (args.Length == 2) Console.WriteLine("v1.1");
            FileInfo inputFileInfo = args.GetFirst<FileInfo>();
            String html = File.ReadAllText(inputFileInfo.FullName);
            String xml = html.HtmlToXml();
            Console.Write(xml);
        }
    }
}
