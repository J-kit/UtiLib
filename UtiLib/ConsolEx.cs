using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib
{
    /// <summary>
    /// Provides extra features simmilar to System.Console
    /// </summary>
    public class Consolex
    {
        /// <summary>
        /// Writes the given string to the default output stream
        /// </summary>
        /// <param name="value"></param>
        public static void Write(string value)
        {
            Console.Write(value);
        }
        /// <summary>
        /// Writes the given string to the default output stream using the color given
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color">The color of the text</param>
        public static void Write(string value, ConsoleColor color)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteLine(string value, params object[] args)
        {
            Console.WriteLine(value, args);
        }
        public static void WriteLine(string value, ConsoleColor color, params object[] args)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value, args);
            Console.ForegroundColor = foregroundColor;

        }
    }
}