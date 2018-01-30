using System;
using System.Threading.Tasks;
using UtiLib.ConsoleTests.Tests;
using UtiLib.Net.Discovery;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Console.WriteLine($"Starting {nameof(PingTests.SafePing)}");
            // PingTests.SafePing();

            //Console.WriteLine($"Starting {nameof(PingTests.RawPingExample)}");
            //PingTests.RawPingExample();
            Console.WriteLine($"Starting {nameof(PingTests.RawPingAsync)}");
            await PingTests.RawPingAsync();

            //Console.WriteLine($"Starting {nameof(PingTests.PingExample)}");
            //PingTests.PingExample();

            //SnifferTests.SnifferExamples();

            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}