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
        //public static T CreateInstance<T>(params object[] args)
        //{
        //    var type = typeof(T);

        //    Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic,null,)

        //    var instance = type.Assembly.CreateInstance(
        //        type.FullName, true,
        //        BindingFlags.Instance | BindingFlags.NonPublic,
        //        null, args, null, null);
        //    return (T)instance;
        //}
        public static T Construct<T>(params object[] paramValues)//Type[] paramTypes,
        {
            Type t = typeof(T);

            var ci = t.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, paramValues.ToArray(m => m.GetType()), null);

            return (T)ci.Invoke(paramValues);
        }

        /*
         *
         * public static T CreateInstance<T>(params object[] args)
        {
            var type = typeof(T);
            var instance = type.Assembly.CreateInstance(
                type.FullName, true,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, args, null, null);
            return (T)instance;
        }
         *
         *
         */
    }
}