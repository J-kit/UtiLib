using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace UtiLib.Serialisation
{
    public class JsonSerializer : ISerializer
    {
        private static JsonSerializer _instance;
        public static JsonSerializer Instance => _instance ?? (_instance = new JsonSerializer());

        public T DeserializeObject<T>(byte[] input) where T : class
        {
            using (var ms = new MemoryStream(input))
            {
                return new DataContractJsonSerializer(typeof(T)).ReadObject(ms) as T;
            }
        }

        public byte[] SerializeObject<T>(T input)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T)).WriteObject(ms, input);
                ms.Position = 0L;
                return ms.ToArray();
            }
        }
    }
}