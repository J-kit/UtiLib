using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UtiLib.Serialisation
{
    public class BinarySerialisation : ISerializer
    {
        public T DeserializeObject<T>(byte[] input) where T : class
        {
            using (var ms = new MemoryStream(input))
            {
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }

        public byte[] SerializeObject<T>(T input)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, input);
                return ms.ToArray();
            }
        }
    }
}