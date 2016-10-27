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

namespace CODEiverse.OST.CommandLineTools.CLBCUriToFile
{

    /// <summary>
    /// Convert a Uri on the command line into a file which is written to standard out
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {

            var uri = args.GetFirst<Uri>();

            var fileName = args.Skip(1)
                               .ToArray()
                               .GetFirst<FileInfo>();

            if (!ReferenceEquals(uri, null))
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                if (!ReferenceEquals(fileName, null))
                {
                    Console.WriteLine("Writing CSV contents to: {0}", fileName.FullName);
                    var utf8Html = wc.DownloadString(uri);
                    File.WriteAllText(fileName.FullName, utf8Html);
                }
                else
                {
                    var utf8Bytes = wc.DownloadData(uri);
                    Console.OutputEncoding = Encoding.UTF8;
                    var output = Console.OpenStandardOutput();
                    output.Write(utf8Bytes, 0, utf8Bytes.Length);
                    output.Close();
                }
            }
            else PrintSyntax("Uri");

        }
    }
}
