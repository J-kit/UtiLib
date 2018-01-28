using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
using UtiLib.Net.Headers;
using UtiLib.Net.Sniffing;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RawPingScan mp = new RawPingScan();
            // mp.OnResult += OnMpOnResult;

            mp.Enqueue(NetMaskGenerator.GetAllIp());

            // mp.Enqueue("10.0.0.138".AsIpAddress());
            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }
    }
}