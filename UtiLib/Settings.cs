using System.Text;
using UtiLib.IO;
using UtiLib.Logging;
using UtiLib.Serialisation;
using UtiLib.Shared.Interfaces;

namespace UtiLib
{
    public class Settings
    {
        private static ILogger _logger;
        private static ISerializer _defaultSerializer;
        private static ISerializer _jsonSerializer;

        public static ISerializer DefaultSerializer
        {
            get => _defaultSerializer ?? JsonSerializer;
            set => _defaultSerializer = value;
        }

        public static ISerializer JsonSerializer
        {
            get => _jsonSerializer ?? (_jsonSerializer = Serialisation.JsonSerializer.Instance);
            set => _jsonSerializer = value;
        }

        public static ISerializer XmlSerializer { get; set; }
        public static ISerializer BinarySerializer { get; set; }

        public static IRingbufferProvider RingbufferProvider { get; set; }

        public static Encoding DefaultEncoding { get; set; }

        public static bool AllowLogBreak { get; set; }

        public static ILogger Logger
        {
            get => _logger ?? (_logger = LogFactory.DefaultLogger);
            set => _logger = value;
        }

        static Settings()
        {
            XmlSerializer = new XmlSerialisation();
            BinarySerializer = new BinarySerialisation();

            RingbufferProvider = new ConcurrentRingBuffer(2);

            DefaultEncoding = Encoding.UTF8;
        }
    }
}