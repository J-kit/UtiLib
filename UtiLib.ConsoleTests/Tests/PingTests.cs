using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UtiLib.Net.Discovery;
using UtiLib.Net.Discovery.Ping;
using UtiLib.Net.Discovery.Tcp;
using UtiLib.Shared.Enums;

namespace UtiLib.ConsoleTests.Tests
{
    internal class PingTests
    {
        public static void SafePing()
        {
            var discoverer =
                NetDiscovery.CreatePingEngine(PingEngineFlags.MeasureTime | PingEngineFlags.Subnet);
            discoverer.OnResult += (_, x) =>
                Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);
            discoverer.OnPingFinished += (_, __) => Logger.Log("Discovery Finished");
            discoverer.TimeOut = TimeSpan.FromSeconds(5);
            discoverer.Start();
            Console.ReadLine();
        }

        public static void PingExample()
        {
            var ps = new ApiPing { TimeOut = TimeSpan.FromSeconds(2) }.Prepare(PingEngineFlags.Subnet);
            ps.OnResult += (sender, address) => Logger.Log(address);
            ps.OnPingFinished += (sender, eventArgs) => Logger.Log("PingScan Finished");
            ps.Start();
            Console.ReadLine();
        }

        public static void RawPingExample()
        {
            using (var mp = new RawPing().Prepare(PingEngineFlags.Subnet))
            {
                mp.OnResult += (_, x) => Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);
                mp.Start();
                Console.ReadLine();
            }
        }

        public static async Task RawPingAsync()
        {
            using (var mp = new RawPing().Prepare(PingEngineFlags.MeasureTime | PingEngineFlags.Subnet))
            {
                mp.OnResult += (_, x) => Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);

                await mp.StartAsync();
            }

            Logger.Log("RawPing Finished");
            Console.ReadLine();
        }

        public static void PortScanTest()
        {
            var tt = new FluidTcpScan();
            var addr = $"10.0.0.138".AsIpAddress();
            var ports = new[] { 1, 80, 443, 380 };
            tt.Enqueue(ports.Select(m => new IPEndPoint(addr, m)));
        }

        public static void CombinedScanTest()
        {
            var fluidScan = new FluidTcpScan();

            var ports = new[] { 80, 443, 3389, 8080, 8000, };
            // var addr = $"10.0.0.138".AsIpAddress();

            using (var mp = new RawPing().Prepare(PingEngineFlags.Subnet))
            {
                mp.OnResult += (_, x) =>
                {
                    Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);
                    fluidScan.Enqueue(ports.Select(m => new IPEndPoint(x.Reply.Address, m)));
                };
                mp.Start();
                Console.ReadLine();
            }
        }
    }
}