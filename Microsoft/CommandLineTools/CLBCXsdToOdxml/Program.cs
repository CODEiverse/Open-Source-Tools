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
//using CODEiverse.OST.Lib.XsltHandling; - Not implemented yet
using System.IO;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Convert an XSD into an ODXML file, which powers the ODXML4 stack.
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {

            throw new NotImplementedException("Neeed to port XsltHandling to be more generic");
/*            String xslt = typeof(Program).Assembly.GetTextResourceContents("XsdToODXML");
            var xhandler = new Lib.XsltHandling.XsltHandler();
            xhandler.LoadXslt(xslt);
            var inputFI = args.GetFirst<FileInfo>();
            xhandler.LoadXmlFromFileInfo(inputFI);
            xhandler.Execute();
            */
        }
    }
}
