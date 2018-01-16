using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UtiLib;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class SerialisationExtensions
    {
        public static T SafeDeepCopy<T>(this T @object) where T : class
        {
            var serialized = Settings.DefaultSerializer.SerializeObject(@object);
            return Settings.DefaultSerializer.DeserializeObject<T>(serialized);
        }

        public static T DeepCopy<T>(this T obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                ms.Position = 0L;
                return (T)bf.Deserialize(ms);
            }
        }

        public static T DeserializeObject<T>(string input) where T : class
        {
            return Settings.DefaultSerializer.DeserializeObject<T>(input.GetBytes());
        }

        public static string SerializeObject<T>(T input)
        {
            return Settings.DefaultSerializer.SerializeObject(input).GetString();
        }
    }
}