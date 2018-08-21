using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Reflection
{
    internal class TypeHelper
    {
        public static T Construct<T>(params object[] paramValues)//Type[] paramTypes,
        {
            Type t = typeof(T);

            var ci = t.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, paramValues.ToArray(m => m.GetType()), null);

            return (T)ci.Invoke(paramValues);
        }
    }
}
