using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Net.Headers;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    public class RawPingNew : PingBase
    {
        private readonly Stopwatch _pingStopwatch;
        private readonly DynamicArray<byte> _payloadData;

        private Socket _socket;

        public RawPingNew() : base()
        {
            _pingStopwatch = Stopwatch.StartNew();
            _payloadData = IcmpPacket.GenerateNew();
        }

        public override void Start()
        {
            base.Start();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);

            while (AddressCollectionQueue.TryDequeue(out var currentAddressSet))
            {
                foreach (var ipEndPoint in currentAddressSet.Select(m => new IPEndPoint(m, 0)))
                {
                    SendPing(ipEndPoint);
                }
            }
        }

        private void SendPing(IPEndPoint ep)
        {
            DynamicArray<byte> packet;

            if (MeasureTime)
            {
                var cms = _pingStopwatch.ElapsedTicks;
                var bcms = BitConverter.GetBytes(cms);
                packet = new IcmpPacket(bcms).GeneratePayload();
            }
            else
            {
                packet = _payloadData;
            }

            _socket.BeginSendTo(packet.Value, 0, packet.Size, SocketFlags.None, ep, null, null); //SocketFlags.None
        }
    }
}