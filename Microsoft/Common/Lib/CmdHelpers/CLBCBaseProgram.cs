using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            String fullSyntax = String.Format("Sytax: {0} {1}", "cmd.exe", syntax);
            Console.WriteLine(fullSyntax);
        }
    }
}
