using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using UtiLib.ConsoleTests.Tests;
using UtiLib.Environment;
using UtiLib.Logging;
using UtiLib.Logging.LogProvider;
using UtiLib.Shared.Enums;
using UtiLib.Delegates;
using UtiLib.Net.Discovery;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var elevated = WindowsUser.IsElevated;

            MyPing mp = new MyPing(x => Settings.Logger.Log($"{x}", LogSeverity.Information));

            mp.EnqueueIp("10.0.0.138".AsIpAddress());
            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}// new ConsoleLogger(), new DebugLogger() 