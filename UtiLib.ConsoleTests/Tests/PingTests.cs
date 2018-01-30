using System;
using System.Threading.Tasks;
using UtiLib.Net.Discovery;
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
    }
}