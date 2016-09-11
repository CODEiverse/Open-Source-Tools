/*****************************
Project:    CODEiverse - Open Source Tools (OST)
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

namespace CODEiverse.OST.CommandLineTools.CLBCUriToFile
{

    /// <summary>
    /// Convert a Uri on the command line into a file which is written to standard out
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            var uri = args.Get<Uri>();
            if (!ReferenceEquals(uri, null))
            {
                WebClient wc = new WebClient();
                Console.WriteLine(wc.DownloadString(uri));
            }
            else PrintSyntax("Uri");

        }
    }
}
