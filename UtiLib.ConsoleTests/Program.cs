using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UtiLib.ConsoleTests.Tests;
using UtiLib.IO;
using UtiLib.Net.Discovery;
using UtiLib.Net.Discovery.Tcp;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            InterfaceForceTest.TestForceInterface();
            Console.ReadLine();

            //   PingTests.CombinedScanTest();
            // Console.WriteLine($"Starting {nameof(PingTests.SafePing)}");
            // PingTests.SafePing();

            //Console.WriteLine($"Starting {nameof(PingTests.RawPingExample)}");
            //PingTests.RawPingExample();
            //   Console.WriteLine($"Starting {nameof(PingTests.RawPingAsync)}");
            //await PingTests.RawPingAsync();

            //Console.WriteLine($"Starting {nameof(PingTests.PingExample)}");
            //PingTests.PingExample();

            Console.ReadLine();
            SnifferTests.SnifferExamples();

            // await TranslateTests.Translate();
            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}