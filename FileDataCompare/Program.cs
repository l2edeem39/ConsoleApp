using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDataCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings["pathOutput"].ToString();
            string[] sources = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["pathSource"].ToString());
            string[] filePaths = Directory.GetFiles(ConfigurationManager.AppSettings["pathDATA_TPA_Daily"].ToString());
            var matchFile = false;
            Console.WriteLine("Please Wait...");

            List<DataTable> dts = new List<DataTable>();
            DataTable table = new DataTable();
            foreach(var item in filePaths)
            {
                var response = FileToTable(item);
                response.TableName = Path.GetFileName(item);
                dts.Add(response);
            }
            
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("{0}", DateTime.Now);
                foreach (string source in sources)
                {
                    matchFile = false;
                    foreach (var item in dts)
                    {
                        var queryAllTable = from resTable in item.AsEnumerable()
                                            where resTable.Field<string>("COL 38") == source
                                            select new
                                            {
                                                Sequence = resTable.Field<string>("COL 1"),
                                                CertificateNo = resTable.Field<string>("COL 38"),
                                                Transtype = resTable.Field<string>("COL 96")
                                            };

                        if (queryAllTable.Count() > 0)
                        {
                            matchFile = true;
                            foreach (var queryTable in queryAllTable)
                            {
                                sw.WriteLine("{0},FOUND,{1},{2},{3}", source, item.TableName, queryTable.Sequence, queryTable.Transtype);
                            }
                        }
                    }

                    if (!matchFile)
                    {
                        //Console.WriteLine("{0},NOT FOUND,,,,", line);
                        sw.WriteLine("{0},NOT FOUND,,,,", source);
                    }
                    Console.WriteLine(source);
                }
            }
            Console.WriteLine("Done.");
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
        private static DataTable FileToTable(String fileName)
        {
            DataTable result = new DataTable();

            var text = System.IO.File.ReadAllText(fileName, Encoding.Default);
            string resulttext = text.Replace("\r", "######").Replace("\n", "******");
            string resultS1 = resulttext.Replace(" ######******|", "").Replace(" ######****** ", "");
            string resultS = resultS1.Replace(" ######******", "\n").Replace("######******1", "\n1").Replace("######******", "");
            var lines = resultS.Split('\n');

            foreach (var line in lines)
            {
                DataRow row = result.NewRow();
                String[] items = line.Replace("\r", "").Split('|'); 
                for (int i = result.Columns.Count; i < items.Length; ++i)
                {
                    result.Columns.Add(String.Format("COL {0}", i + 1));
                }
                row.ItemArray = items;
                result.Rows.Add(row);
            }
            //abc
            return result;
        }
    }
}
