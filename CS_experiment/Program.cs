using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_experiment
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now + " - Start");

            Sources.tableHandler.xml_to_sql(
                    @"C:\Users\Nongi\Documents\GitHub\CS_experiment\CS_experiment\Sources\tables_exemple.xml",
                    @"C:\Users\Nongi\Desktop\sql_query.sql");

            Console.WriteLine(DateTime.Now + " - End");

            //Console.ReadLine();
        }
    }
}
