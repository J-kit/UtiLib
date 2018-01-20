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
            //var interfaces = NetMaskGenerator.GetInterfaces();
            //foreach (var ni in interfaces)
            //{
            //    Logger.Log($"{ni.Name}: {ni.Address}");

            //    //Logger.Log();
            //}
            //    Debugger.Break();
            var sniffer = new RawSniffer { NetworkAdapter = "10.0.0.16".AsIpAddress() };
            sniffer.OnUnknownPacketCaptured += (_, __) => Logger.Log($"Unknown packet received");
            sniffer.OnPacketCaptured += OnSnifferOnOnPacketCaptured;

            sniffer.FilteringRule = x => x.Body.SourcePort == 53 || x.Body.DestinationPort == 53;//x.Header.ProtocolType == ProtocolType.Udp;

            sniffer.Start();
            Logger.Log($"Sniffing started");
            Console.ReadLine();
            var elevated = WindowsUser.IsElevated;

            //foreach (var ipAddress in NetMaskGenerator.GetAllIp())
            //{
            //    Console.WriteLine(ipAddress);
            //}

            RawPingDiscovery mp = new RawPingDiscovery();
            // mp.OnResult += OnMpOnResult;

            mp.Enqueue(NetMaskGenerator.GetAllIp());

            // mp.Enqueue("10.0.0.138".AsIpAddress());
            Console.ReadLine();
            DelegatePlayground.CreateDelegate();
            LogTests.DoTest();
            Console.ReadLine();
        }

        private static void OnSnifferOnOnPacketCaptured(object _, SniffingPacket x)
        {
            if (x.DnsInfo == null)
            {
                Logger.Log($"({x.Header.Data.Length}/{x.Body.Data.Length})" +
                           $"{x.Header.ProtocolType} packet: " +
                           $"{x.Header.SourceAddress}:{x.Body.SourcePort} ===> " +
                           $"{x.Header.DestinationAddress}:{x.Body.DestinationPort}");
            }
            else
            {
                Debugger.Break();
            }
        }

        //private static void OnMpOnResult(object _, IcmpPacket packet)
        //{
        //    Settings.Logger.Log($"{packet.IpHeader.SourceAddress}: {packet.IpStatus}", LogSeverity.Information);
        //}
    }
}// new ConsoleLogger(), new DebugLogger() 