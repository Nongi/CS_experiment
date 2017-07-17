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

            List<Sources.Table_Structure> l_tbl = new List<Sources.Table_Structure>();
            l_tbl.Add(new Sources.Table_Structure(0, @"G:\SD.ZSSIAD.TAZCACC.DDL", "TMPDAY"));

            Sources.tableHandler.struct_to_xml(l_tbl, @"C:\Users\Nongi\Documents\GitHub\CS_experiment\CS_experiment\Sources\tables_exemple_bis.xml");

            Sources.tableHandler.xml_to_sql(
                    @"C:\Users\Nongi\Documents\GitHub\CS_experiment\CS_experiment\Sources\tables_exemple_bis.xml",
                    @"C:\Users\Nongi\Desktop\sql_query.sql");
            Console.WriteLine(DateTime.Now + " - End");

            //Console.ReadLine();
        }
    }
}
