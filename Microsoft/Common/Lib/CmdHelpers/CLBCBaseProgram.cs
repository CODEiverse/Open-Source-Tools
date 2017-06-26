using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace CODEiverse.OST.Lib.CmdHelpers
{
    public class CLBCBaseProgram
    {
        /// <summary>
        /// Print the syntax for the current command line tool
        /// </summary>
        /// <param name="syntax">The syntax</param>
        public static void PrintSyntax(String syntax)
        {
            string codeBaseUri = Assembly.GetEntryAssembly().CodeBase;
            string codeBase = Path.GetFileName(new Uri(codeBaseUri).LocalPath);
            String fullSyntax = String.Format("Sytax: {0} {1}", codeBase, syntax);
            Console.WriteLine(fullSyntax);
        }




    }
}
