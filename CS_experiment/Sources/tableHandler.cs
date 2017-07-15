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
        public static string xml2sql(string xml_path)
        {
            using (XmlReader reader = XmlReader.Create(xml_path))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    Console.WriteLine(reader.Value);
                }
            }
            return "";
        }
    }
}
