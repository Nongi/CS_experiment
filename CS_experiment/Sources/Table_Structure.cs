using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_experiment.Sources
{

    /// <summary>
    /// Class to describe DB table
    /// </summary>
    public class Table_Structure
    {
        /// <summary>Table's name</summary>
        /// <remarks> The value must be string and not null.</remarks>
        public string name;

        /// <summary>Table's schema name</summary>
        /// <remarks> The value must be string and not null.</remarks>
        public string schema;

        /// <summary>List containing tuples(name, type, length) representing the fields of the table</summary>
        public List<Tuple<string, string, string>> structure;

        /// <summary>Table's keys</summary>
        public List<string> keys;

        public Table_Structure()
        {
            name = "";
            schema = "";
            structure = new List<Tuple<string, string, string>>();
            keys = new List<string>();
        }

        public Table_Structure(string name_in, string schema_in)
        {
            name = name_in;
            schema = schema_in;
            structure = new List<Tuple<string, string, string>>();
            keys = new List<string>();

        }
    }
}
