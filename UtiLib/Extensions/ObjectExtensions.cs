using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.IO;

namespace UtiLib.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Casts the input object to <see cref="T"/>, even if <see cref="input"/> doesn't deriver from <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T ForceCast<T>(this object input) where T : class
        {
            var interfaceType = input.GetType();
            if (typeof(T).IsAssignableFrom(interfaceType))
            {
                return (T)input;
            }

            return new ImplementationWrappingProxy<T>(input).Create();
        }
    }
}