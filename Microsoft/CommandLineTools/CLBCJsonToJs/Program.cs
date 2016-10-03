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
    /// Convert a Json file into a JS file.  Adds var filename = + json.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            if (!args.Any()) Console.Write("Sytnax: CLBCJsonToJs FileName.json");
            else
            {
                FileInfo fi = args.GetFirst<FileInfo>();
                if (!fi.Exists) throw new Exception(String.Format("File {0} does not exist.", fi.FullName));
                String jsFileName = String.Format("{0}.js", fi.FullName);
                String objectName = fi.Name.Substring(0, fi.Name.IndexOf(".") - 1);

                var json = File.ReadAllText(fi.FullName);
                String js = String.Format("var {0} = {1}", objectName, json);
                File.WriteAllText(jsFileName, js);
                Console.WriteLine(String.Format("File {0} generated.", jsFileName));
            }
        }
    }
}
