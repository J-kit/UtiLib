using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib
{
    public class Consolex
    {
        public static void Write(string value)
        {
            Console.Write(value);
        }

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
    }
}