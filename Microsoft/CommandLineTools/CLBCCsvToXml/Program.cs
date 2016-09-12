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
using System.Data.OleDb;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace CODEiverse.OST.CommandLineTools
{

    /// <summary>
    /// Convert the Csv file into an Xml file
    /// </summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
            if (!args.Any()) Console.Write("Sytnax: CLBCCsvToXml FileName.csv");
            else
            {
                FileInfo fi = new FileInfo(args[0]);
                if (!fi.Exists) throw new Exception(String.Format("File {0} does not exist.", fi.FullName));
                string FileName = fi.FullName;

                OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path.GetDirectoryName(FileName) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");

                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + Path.GetFileName(FileName), conn);
                var pluralizer = PluralizationService.CreateService(CultureInfo.CurrentCulture);
                var fileName = Path.GetFileNameWithoutExtension(fi.Name);
                var pluralFileName = pluralizer.Pluralize(fileName);
                var singularFileName = pluralizer.Singularize(fileName);
                DataSet ds = new DataSet(pluralFileName);
                adapter.FillSchema(ds, SchemaType.Mapped);
                var table = ds.Tables[0];
                foreach (DataColumn col in table.Columns)
                {
                    col.DataType = typeof(String);
                }
                adapter.Fill(ds);
                conn.Close();

                table.TableName = singularFileName;
                FileStream fs = new FileStream(String.Format("{0}.xml", FileName), FileMode.Create);

                table.WriteXml(fs);
                fs.Close();
            }
        }
    }
}
