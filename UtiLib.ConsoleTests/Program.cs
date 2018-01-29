using System;
using UtiLib.ConsoleTests.Tests;
using UtiLib.Net.Discovery;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Starting {nameof(PingTests.RawPingExample)}");
            PingTests.RawPingExample();
            Console.WriteLine($"Starting {nameof(PingTests.PingExample)}");
            PingTests.PingExample();

            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}