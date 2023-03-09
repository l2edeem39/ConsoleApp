using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchCertificateFileData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please Wait...");
            string path = ConfigurationManager.AppSettings["pathFile"].ToString();
            string pathOutput = ConfigurationManager.AppSettings["pathOutput"].ToString().Replace(".txt", "") + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
            string[] excelPath = Directory.GetFiles(path.ToString());
            string[] sources = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["pathSource"].ToString());
            List<DataTable> listDtNew = new List<DataTable>();
            var listDict = new List<Dictionary<string, string>>();
            foreach (var itemFile in excelPath)
            {
                var res = FileToDict(itemFile);
                listDict.Add(res);
            }
            var index = 0;
            using (StreamWriter sw = File.CreateText(pathOutput))
            {
                sw.WriteLine("{0}", DateTime.Now);
                foreach (var source in sources)
                {
                    index++;
                    var matchFile = false;
                    foreach (var item in listDict)
                    {
                        if (item.ContainsKey(source))
                        {
                            matchFile = true;
                            //Console.WriteLine(item[source]+"=>"+index);
                            sw.WriteLine("{0},FOUND,{1}",source, item[source]);
                        }
                    }

                    if (matchFile)
                    {
                        Console.WriteLine("{0} FOUND => {1}", source, index);
                    }
                    else
                    {
                        sw.WriteLine("{0},NOT FOUND,,,,", source);
                        Console.WriteLine("{0} NOT FOUND => {1}", source, index);
                    }
                }
            }
            Console.WriteLine("Done.");
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
        private static Dictionary<string, string> FileToDict (String pathFile)
        {
            var result = new Dictionary<string, string>();
            FileStream stream = File.Open(pathFile, FileMode.Open, FileAccess.Read);

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                while (reader.Read())
                {
                    result[reader[12].ToString()] = String.Format("{0},{1},{2}", reader[1].ToString(), reader[6].ToString(), Path.GetFileName(pathFile));
                }
            }
            return result;
        }
    }
}
