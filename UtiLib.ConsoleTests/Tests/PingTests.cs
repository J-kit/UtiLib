using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Net.Discovery;
using UtiLib.Shared.Enums;

namespace UtiLib.ConsoleTests.Tests
{
    internal class PingTests
    {
        public static void PingExample()
        {
            var ps = new ApiPing { TimeOut = TimeSpan.FromSeconds(2) };
            ps.OnResult += (sender, address) => Logger.Log(address);
            ps.OnPingFinished += (sender, eventArgs) => Logger.Log("PingScan Finished");
            ps.Enqueue(NetMaskGenerator.GetAllIp());
            ps.Start();
            Console.ReadLine();
        }

        public static void RawPingExample()
        {
            using (var mp = new RawPing())
            {
                mp.OnResult += (_, x) => Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);

                mp.Enqueue(NetMaskGenerator.GetAllIp());
                mp.Start();
                Console.ReadLine();
            }
        }
    }
}