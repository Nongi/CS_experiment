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

            Sources.tableHandler.xml2sql(@"C:\Users\Nongi\Documents\GitHub\CS_experiment\CS_experiment\Sources\tables_exemple.xml");

            Console.WriteLine(DateTime.Now + " - End");
            Console.ReadLine();
        }
    }
}
