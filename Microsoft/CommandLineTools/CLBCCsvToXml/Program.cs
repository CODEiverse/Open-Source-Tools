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
            if (!args.Any())
            {
                Console.WriteLine("Sytnax: CLBCCsvToXml FileName.csv");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
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

                table.Rows
                    .OfType<DataRow>()
                    .ToList()
                    .ForEach(feRow => CleanRow(feRow));

                String xmlFileName = String.Format("{0}.xml", FileName);
                FileStream fs = new FileStream(xmlFileName, FileMode.Create);

                table.WriteXml(fs);
                fs.Close();
                String fileText = File.ReadAllText(xmlFileName);
                File.WriteAllText(xmlFileName, fileText.Replace("&lt;![CDATA_START[", "<![CDATA[").Replace("]CDATA_END]&gt;", "]]>"));
            }
        }

        private static void CleanRow(DataRow feRow)
        {
            feRow.Table
                 .Columns
                 .OfType<DataColumn>()
                 .ToList()
                 .ForEach(feCol => CleanCol(feRow, feCol));
        }

        private static void CleanCol(DataRow feRow, DataColumn feCol)
        {
            var value = feRow[feCol].SafeToString();
            if (value.Any(anyChar => !Char.IsLetterOrDigit(anyChar) && !" ?~`!@#$%^&*()-_=+[{]}\\|;:',./?".Contains(anyChar)))
            {
                feRow[feCol] = String.Format("<![CDATA_START[{0}]CDATA_END]>", value);
                feRow.AcceptChanges();
            }
        }
    }
}
