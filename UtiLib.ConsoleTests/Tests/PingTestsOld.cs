using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Net.Discovery;
using UtiLib.Shared.Enums;

namespace UtiLib.ConsoleTests.Tests
{
    internal class PingTestsOld
    {
        public static void PingExample()
        {
            var ps = new PingScan { TimeOut = TimeSpan.FromSeconds(2) };
            ps.OnResult += (sender, address) => Logger.Log(address);
            ps.OnPingFinished += (sender, eventArgs) => Logger.Log("PingScan Finished");
            ps.Enqueue(NetMaskGenerator.GetAllIp());
            Console.ReadLine();
        }

        public static void RawPingExample()
        {
            RawPingScan mp = new RawPingScan();
            mp.OnResult += (_, x) => Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);

            mp.Enqueue(NetMaskGenerator.GetAllIp());

            // mp.Enqueue("10.0.0.138".AsIpAddress());
            Console.ReadLine();
        }
    }

    internal class PingTestsNew
    {
        public static void PingExample()
        {
            var ps = new ApiPingNew { TimeOut = TimeSpan.FromSeconds(2) };
            ps.OnResult += (sender, address) => Logger.Log(address);
            ps.OnPingFinished += (sender, eventArgs) => Logger.Log("PingScan Finished");
            ps.Enqueue(NetMaskGenerator.GetAllIp());
            Console.ReadLine();
        }

        public static void RawPingExample()
        {
            var mp = new RawPingNew();
            mp.OnResult += (_, x) => Logger.Log($"{x.Reply.Address}: {x.Reply.Status}", LogSeverity.Information);

            mp.Enqueue(NetMaskGenerator.GetAllIp());
            mp.Start();
            Console.ReadLine();
        }
    }
}