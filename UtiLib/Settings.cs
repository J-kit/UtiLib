using System.Text;
using UtiLib.Logging;
using UtiLib.Serialisation;
using UtiLib.Shared.Interfaces;

namespace UtiLib
{
    public class Settings
    {
        private static ILogger _logger;

        public static ISerializer DefaultSerializer { get; set; }
        public static ISerializer JsonSerializer { get; set; }
        public static ISerializer XmlSerializer { get; set; }
        public static ISerializer BinarySerializer { get; set; }

        public static Encoding DefaultEncoding { get; set; }

        public static bool AllowLogBreak { get; set; }

        public static ILogger Logger
        {
            get => _logger ?? (_logger = LogFactory.DefaultLogger);
            set => _logger = value;
        }

        static Settings()
        {
            JsonSerializer = DefaultSerializer = new JsonSerializer();
            XmlSerializer = new XmlSerialisation();
            BinarySerializer = new BinarySerialisation();

            DefaultEncoding = Encoding.UTF8;
        }
    }
}