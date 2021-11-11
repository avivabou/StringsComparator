using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace StringsComparator
{
    class Program
    {
        static void Main(string[] args)
        {
            string str1 = "91111111111111111111111112901129129xxxxxxx";
            string str2 = "1129000011110000001293333333333333333333333";
            //string str2 = "12345";
            //string str1 = "12345123451234512345";
            DynamicComparatorTable dct = new DynamicComparatorTable(str1, str2);
            dct.Initialize();
            dct.FillTable();
            dct.PrintTable();
            dct.PrintCommonChars();
            dct.PrintDiffrences();
            //DoCalculation();
            Console.ReadLine();
        }

        static void DoCalculation()
        {
            using (StreamReader sr = new StreamReader(@"C:\Users\AvivAbou\OneDrive - Global-e\Desktop\TPU\Feed.csv"))
            {
                string line = sr.ReadLine();
                line = sr.ReadLine();
                while (line != null)
                {
                    TextFieldParser parser = new TextFieldParser(new StringReader(line));
                    parser.SetDelimiters(",");
                    string[] rawFields = parser.ReadFields();
                    Logger.Log.WriteLine(rawFields[0]+" -> "+rawFields[1]);
                    CalcDiffrences(rawFields[2], rawFields[3]);
                    Logger.Log.WriteLine("-------------------------------------------------");
                    line = sr.ReadLine();
                }
            }
        }


        static void CalcDiffrences(string str1, string str2)
        {
            ComparatorTableSkeleton dt = new DynamicComparatorTable(str1, str2);
            dt.Initialize();
            dt.FillTable();
            //dt.PrintSeqs();
            dt.PrintDiffrences();
            Logger.Log.WriteLine("");
        }
    }
}
