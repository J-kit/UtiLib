using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace UtiLib.Serialisation
{
    public static class SerialisationExtensions
    {
        public static string AsJson(this object target)
        {
            return Settings.JsonSerializer.SerializeObject(target).GetString();
        }

        public static T Deserialize<T>(this string target) where T : class
        {
            return Settings.JsonSerializer.DeserializeObject<T>(target.GetBytes());
        }
    }
}