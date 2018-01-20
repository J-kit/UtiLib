using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Returns true when all <see cref="options"/> are eqal to <see cref="reference"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MultiEquals<T>(this T reference, params T[] options)
        {
            for (var i = 0; i < options.Length; i++)
            {
                if (!i.Equals(reference))
                {
                    return false;
                }
            }

            return true;
        }
    }
}