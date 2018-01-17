using System;
using System.IO;
using System.Xml.Serialization;

namespace UtiLib.Serialisation
{
    public class XmlSerialisation : ISerializer
    {
        public T DeserializeObject<T>(byte[] input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(input.GetString()))
                return serializer.Deserialize(reader) as T;
        }

        public byte[] SerializeObject<T>(T input)
        {
            using (var ms = new MemoryStream())
            {
                new XmlSerializer(typeof(T)).Serialize(ms, input);
                ms.Position = 0L;

                return ms.ToArray();
            }
        }
    }
}