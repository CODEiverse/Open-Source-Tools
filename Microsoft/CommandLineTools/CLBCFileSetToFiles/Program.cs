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
    /// Description of the tool here
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(15000);
            String fileName = args.GetFirst<String>();
            String fileSetXml = String.Empty;
            String directory = Environment.CurrentDirectory;
            if (String.IsNullOrEmpty(fileName))
            {
                bool isKeyAvailable;

                try
                {
                    isKeyAvailable = System.Console.KeyAvailable;
                }
                catch (InvalidOperationException expected)
                {
                    fileSetXml = System.Console.In.ReadToEnd();
                }
            }
            else
            {
                directory = Path.GetDirectoryName(fileName);
                fileSetXml = File.ReadAllText(fileName);
            }

            Console.Error.WriteLine(directory);

            SplitContents(fileSetXml, false, directory);
        }
    }
}
