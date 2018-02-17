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
        public static bool MultiEqualsAnd<T>(this T reference, params T[] options)
        {
            return options.All(t => t.Equals(reference));
        }

        /// <summary>
        /// Returns true when all <see cref="options"/> are eqal to <see cref="reference"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MultiEqualsOr<T>(this T reference, params T[] options)
        {
            return options.Contains(reference);
        }

        /// <summary>
        /// Returns the string representation of the bool in lower capitals
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToLower(this bool input)
        {
            return input.ToString().ToLower();
        }

        /// <summary>
        /// Returns the string representation of the bool in upper capitals
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUpper(this bool input)
        {
            return input.ToString().ToUpper();
        }
    }
}