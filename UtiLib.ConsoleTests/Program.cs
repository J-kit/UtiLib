using System;
using UtiLib.ConsoleTests.Tests;
using UtiLib.Net.Discovery;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Starting {nameof(PingTestsNew.RawPingExample)}");
            PingTestsNew.RawPingExample();
            Console.WriteLine($"Starting {nameof(PingTestsNew.PingExample)}");
            PingTestsNew.PingExample();

            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}