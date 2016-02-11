#region Imports

//C# imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

#endregion

namespace FunWithDataAccess
{
    class ImTheSerializerGeek
    {
        public static string SerializeToXML(Person p)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Person));
            string xml;

            using (StringWriter sw = new StringWriter())
            {                
                ser.Serialize(sw, p);
                xml = sw.ToString();
            }

            return xml;
        }

        public static Person DeserializeFromXML(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Person));
            using (StringReader sr = new StringReader(xml))
            {
                Person p = (Person)ser.Deserialize(sr);

                return p;
            }
        }

    }

    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [XmlIgnore]
        public int Age { get; set; }
    }
}
