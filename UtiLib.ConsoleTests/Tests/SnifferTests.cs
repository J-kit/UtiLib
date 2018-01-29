using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Environment;
using UtiLib.Net.Discovery;
using UtiLib.Net.Sniffing;

namespace UtiLib.ConsoleTests.Tests
{
    internal class SnifferTests
    {
        public static void SnifferExamples()
        {
            Logger.Log($"Is Elevated: {WindowsUser.IsElevated}");
            Logger.Log("Listing Network Adapters...");
            var interfaces = NetMaskHelper.GetInterfaces();
            foreach (var ni in interfaces)
            {
                Logger.Log($"{ni.Name}: {ni.Address}");
            }

            Logger.Log("Starting socket");
            var sniffer = new RawSniffer { NetworkAdapter = "10.0.0.16".AsIpAddress() };//
            sniffer.OnUnknownPacketCaptured += (_, __) => Logger.Log($"Unknown packet received");
            sniffer.OnPacketCaptured += OnSnifferOnOnPacketCaptured;

            //sniffer.FilteringRule = x => x.Body.SourcePort == 53 || x.Body.DestinationPort == 53;//x.Header.ProtocolType == ProtocolType.Udp;

            sniffer.Start();
            Logger.Log($"Sniffing started");
            Console.ReadLine();
        }

        private static void OnSnifferOnOnPacketCaptured(object _, SniffingPacket x)
        {
            if (x.DnsInfo == null)
            {
                Logger.Log($"({x.Header.Data.Length}/{x.Body.Data.Count})" +
                           $"{x.Header.ProtocolType} packet: " +
                           $"{x.Header.SourceAddress}:{x.Body.SourcePort} ===> " +
                           $"{x.Header.DestinationAddress}:{x.Body.DestinationPort}");
            }
            else
            {
                // Debugger.Break();
            }
        }
    }
}