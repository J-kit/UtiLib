using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Serialisation;

namespace UtiLib
{
    public class Settings
    {
        public static ISerializer DefaultSerializer { get; set; }
        public static ISerializer DefaultXmlSerializer { get; set; }
        public static ISerializer DefaultJsonSerializer { get; set; }

        public static Encoding DefaultEncoding { get; set; }

        static Settings()
        {
            DefaultJsonSerializer = DefaultSerializer = new JsonSerializer();
            DefaultXmlSerializer = new XmlSerialisation();
            DefaultEncoding = Encoding.UTF8;
        }
    }
}