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

        public Table_Structure(int input_type, string path_file, string schema_in)
        {
            name = "";
            schema = schema_in;
            structure = new List<Tuple<string, string, string>>();
            keys = new List<string>();

            var ddl_tmp = get_DDL_string(path_file);
            var lines = get_DDL_lines(ddl_tmp);

            for (int i = 0; i < lines.Count(); i++)
            {
                String[] line_ = lines[i];

                if (line_.Count() > 0)
                {
                    switch (line_[1])
                    {
                        case "TABLE":
                            name = line_[2].Substring(line_[2].IndexOf('.') + 1);
                            break;
                        case "CHAR":
                            structure.Add(new Tuple<String, String, String>(line_[0], "char", line_[2]));
                            break;
                        case "SMALLINT":
                            structure.Add(new Tuple<String, String, String>(line_[0], "smallint", "5"));
                            break;
                        case "INTEGER":
                            structure.Add(new Tuple<String, String, String>(line_[0], "integer", "10"));
                            break;
                        case "DECIMAL":
                            structure.Add(new Tuple<String, String, String>(line_[0], "decimal", line_[2] + line_[3]));
                            break;
                        case "DATE":
                            structure.Add(new Tuple<String, String, String>(line_[0], "date", "10"));
                            break;
                        case "TIME":
                            structure.Add(new Tuple<String, String, String>(line_[0], "time", "8"));
                            break;
                        case "TIMESTAMP":
                            if (line_[2] == "EXTERNAL")
                                structure.Add(new Tuple<String, String, String>(line_[0], "timestamp", "19"));
                            else
                                structure.Add(new Tuple<String, String, String>(line_[0], "timestamp", "26"));
                            break;
                        case "KEY":
                            /*
                            if (line_.Length > 2)
                                primary_key.Add(line_[2]);

                            while (lines[i + 1].Count() == 1) {
                                i++;
                                //primary_key += lines[i][0];
                            }*/

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public List<String[]> get_DDL_lines(string ddl_tmp)
        {
            List<String[]> lines = new List<string[]>();

            foreach (string ddl_line in ddl_tmp.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] line_values = ddl_line.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                if (line_values.Length != 0)
                    lines.Add(line_values);
            }

            lines.RemoveAt(lines.Count() - 1);

            return lines;
        }

        public string get_DDL_string(string path_file)
        {
            string line;
            String ddl_tmp = "";

            System.IO.StreamReader file = new System.IO.StreamReader(path_file, Encoding.GetEncoding("iso-8859-1"));
            while ((line = file.ReadLine()) != null)
            {
                ddl_tmp += line + "\n";
            }
            file.Close();

            ddl_tmp = ddl_tmp.Substring(ddl_tmp.IndexOf("CREATE TABLE "));
            ddl_tmp = ddl_tmp.Remove(ddl_tmp.IndexOf("COMMENT"));

            return ddl_tmp;
        }

    }
}
