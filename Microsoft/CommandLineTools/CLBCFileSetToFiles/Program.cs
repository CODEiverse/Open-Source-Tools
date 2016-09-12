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
using System.Threading;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Splits the file passed (or piped in) into the files specified by the relative path.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(15000);
            String fileName = args.GetFirst<String>();
            FileInfo fi = new FileInfo(fileName);
            String fileSetXml = String.Empty;
            String directory = Environment.CurrentDirectory;
            if (String.IsNullOrEmpty(fileName)) PrintSyntax("FileSet.xml");
            else
            {
                directory = fi.Directory.FullName;
                fileSetXml = File.ReadAllText(fileName);
            }

            SplitContents(fileSetXml, false, directory);
        }
    }
}
