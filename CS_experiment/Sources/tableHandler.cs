using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CS_experiment.Sources
{
    public class tableHandler
    {
        /// <summary>
        /// Methods to convert a xml file which describe data to sql queries 
        /// </summary>
        /// <param name="xml_path">xml which describe the data</param>
        /// <param name="sql_path">sql file that will contain create SQL queries</param>
        public static void xml_to_sql(string xml_path, string sql_path)
        {
            //Clean the output file
            System.IO.File.WriteAllText(sql_path, string.Empty);

            //Open the xml with all the table
            using (XmlReader xmlReader = XmlReader.Create(xml_path))
            {
                //Read the xml file
                while (xmlReader.Read())
                {
                    //Processing only when we find the "table" element's start 
                    if (xmlReader.IsStartElement() && xmlReader.Name == "table")
                    {
                        try
                        {
                            Table_Structure tbl_struct_tmp = new Table_Structure(xmlReader["name"], xmlReader["schema"]);

                            //Add all element's field to the Table_Structure after verifying that we have name/type/length
                            while (xmlReader.Read() && xmlReader.Name != "table" && xmlReader.NodeType != XmlNodeType.EndElement)
                            {
                                if (xmlReader.Name == "field" 
                                    && xmlReader["name"] != null 
                                    && xmlReader["type"] != null 
                                    && xmlReader["length"] != null)
                                {
                                    tbl_struct_tmp.structure.Add(new Tuple<string, string, string>(
                                        xmlReader["name"], 
                                        xmlReader["type"], 
                                        xmlReader["length"]));
                                }
                            }

                            var sql_create = get_vertica_ddl_varchar(tbl_struct_tmp);
                            //var sql_create = get_vertica_ddl_varchar(tbl_struct_tmp);
                            append_sql_to_file(sql_path, sql_create);
                        }
                        catch (Exception err)
                        {
                            //TODO : gérer un fichier de rejet des tables "out"
                            Console.WriteLine(err);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Methods to convert a list of Table_Structure which describe data to sql queries 
        /// </summary>
        /// <param name="list_table_structure">list which contains all the Table_Structure</param>
        /// <param name="sql_path">sql file that will contain create SQL queries</param>
        public static void struct_to_sql(List<Table_Structure> list_table_structure, string sql_path)
        {
            //Clean the output file
            System.IO.File.WriteAllText(sql_path, string.Empty);
            try
            {
                foreach(Table_Structure tbl_struct_tmp in list_table_structure)
                {
                    var sql_create = get_vertica_ddl_varchar(tbl_struct_tmp);
                    //var sql_create = get_vertica_ddl_varchar(tbl_struct_tmp);
                    append_sql_to_file(sql_path, sql_create);
                }
            }
            catch (Exception err)
            {
                //TODO : gérer un fichier de rejet des tables "out"
                Console.WriteLine(err);
            }
        }

        /// <summary>
        /// Methods to convert a list of Table_Structure which describe data to xml file
        /// </summary>
        /// <param name="list_table_structure">list which contains all the Table_Structure</param>
        /// <param name="xml_path">xml file that will contain data structure</param>
        public static void struct_to_xml(List<Table_Structure> list_table_structure, string xml_path)
        {
            //Clean the output file
            System.IO.File.WriteAllText(xml_path, string.Empty);

            using (XmlWriter writer = XmlWriter.Create(xml_path))
            {
                writer.WriteStartDocument();

                ///
                writer.WriteStartElement("tables");

                foreach (Table_Structure tbl_struct_tmp in list_table_structure)
                {
                    writer.WriteStartElement("table");
                    writer.WriteAttributeString("name", tbl_struct_tmp.name);
                    writer.WriteAttributeString("schema", tbl_struct_tmp.schema);

                    foreach (Tuple<string, string, string> field in tbl_struct_tmp.structure)
                    {
                        writer.WriteStartElement("field");
                        writer.WriteAttributeString("name", field.Item1);
                        writer.WriteAttributeString("type", field.Item2);
                        writer.WriteAttributeString("length", field.Item3);
                        writer.WriteEndElement();
                    }
                }
            }
        }

        /// <summary>
        /// Use the class Table_Structure to create the create query on SQL
        /// WARNING all fields are cast on VARCHAR to make load easier (If not wanted, please see get_vertica_ddl(Table_Structure tbl_struct))
        /// WARNING my DB target is Vertica, the length are on octet and UTF-8 so I multiply by 4 to take into account all the characters
        /// </summary>
        /// <param name="tbl_struct">Table's structure with table's name, table's schema and all fields</param>
        /// <returns>The create query on SQL for the input structure</returns>
        public static string get_vertica_ddl_varchar(Table_Structure tbl_struct)
        {
            string ddl_tmp = "CREATE TABLE " + tbl_struct.schema + "." + tbl_struct.name + " (";

            List<string> list_field_tmp = new List<string>();

            foreach (Tuple<String, String, String> t in tbl_struct.structure)
            {
                switch(t.Item2.ToUpper())
                {
                    case "CHAR":
                    case "VARCHAR":
                        list_field_tmp.Add(t.Item1 + " VARCHAR(" + Int32.Parse(t.Item3) * 4 + ")");
                        break;

                    case "DECIMAL":
                        String[] s_tmp = t.Item3.Split(new[] { ',' });
                        list_field_tmp.Add((Int32.Parse(s_tmp[0]) + Int32.Parse(s_tmp[1]) + 2).ToString());
                        break;

                    default:
                        list_field_tmp.Add(t.Item1 + " VARCHAR(" + t.Item3 + ")");
                        break;
                }
            }

            ddl_tmp += "\r\n\t" + String.Join(",\r\n\t", list_field_tmp) + "\r\n);\n";

            return ddl_tmp;
        }

        /// <summary>
        /// Use the class Table_Structure to create the create query on SQL
        /// WARNING my DB target is Vertica, the length are on octet and UTF-8 so I multiply by 4 to take into account all the characters
        /// </summary>
        /// <param name="tbl_struct">Table's structure with table's name, table's schema and all fields</param>
        /// <returns>The create query on SQL for the input structure</returns>
        public static string get_vertica_ddl(Table_Structure tbl_struct)
        {
            List<string> list_field_tmp = new List<string>();
            foreach (Tuple<string, string, string> t in tbl_struct.structure)
            {
                //TODO : check vertica type
                switch (t.Item2)
                {
                    case "smallint":
                        list_field_tmp.Add(t.Item1 + " smallint");
                        break;
                    case "integer":
                        list_field_tmp.Add(t.Item1 + " INT");
                        break;
                    case "date":
                        list_field_tmp.Add(t.Item1 + " DATE");
                        break;
                    case "time":
                        list_field_tmp.Add(t.Item1 + " TIME");
                        break;
                    case "timestamp":
                        list_field_tmp.Add(t.Item1 + " TIMESTAMP");
                        break;
                    case "char":
                        list_field_tmp.Add(t.Item1 + " VARCHAR(" + Int32.Parse(t.Item3) * 4 + ")");
                        break;
                    case "varchar":
                        list_field_tmp.Add(t.Item1 + " VARCHAR(" + Int32.Parse(t.Item3) * 4 + ")");
                        break;
                    default:
                        list_field_tmp.Add(t.Item1 + ' ' + t.Item2 + '(' + t.Item3 + ")");
                        break;
                }

            }

            string ddl_tmp = "CREATE TABLE " + tbl_struct.schema + "." + tbl_struct.name + " (";
            ddl_tmp += "\r\n\t" + String.Join(",\r\n\t", list_field_tmp);

            if (tbl_struct.keys.Count > 0)
            {
                ddl_tmp += "\r\n\tPRIMARY KEY (" + String.Join(",\r\n\t\t", tbl_struct.keys) + ")";
            }

            return ddl_tmp + "\r\n);\n";
        }

        /// <summary>
        /// Methods to append the create query on SQL to the file given on parameter
        /// </summary>
        /// <param name="sql_path">SQL file we want to fill with all the create query</param>
        /// <param name="sql_query">String which contain the create query</param>
        public static void append_sql_to_file(string sql_path, string sql_query)
        {
            System.IO.File.AppendAllText(sql_path, sql_query);
        }
    }

}
