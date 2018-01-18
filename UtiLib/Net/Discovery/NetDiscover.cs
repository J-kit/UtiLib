using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using UtiLib.Net.Headers;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    internal class NetDiscover
    {
    }

    public class MyPing
    {
        private Action<IPAddress> _onPingReceive;
        private IcmpPacket _packet;// = new IcmpPacket();
        private Socket _socket;//= new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        private int _intCount = 0;

        private readonly DynamicArray<byte> _payloadData;

        public MyPing(Action<IPAddress> onPingReceive)
        {
            _onPingReceive = onPingReceive;
            _packet = new IcmpPacket();

            _payloadData = _packet.GeneratePayload();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        }

        public void EnqueueIp(IPAddress input)
        {
            EndPoint epServer = (new IPEndPoint(input, 0));
            _socket.BeginSendTo(_payloadData.Value, 0, _payloadData.Size, SocketFlags.None, epServer, null, null);

            if (_intCount < 20)
            {
                var receiveBuffer = new byte[256];
                _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, receiveBuffer);
                _intCount++;
            }
        }

        private void ReceiveCallback(IAsyncResult x)
        {
            var receiveBuffer = x.AsyncState as byte[];
            if (_onPingReceive != null && receiveBuffer != null)
            {
                var bytesRead = _socket.EndReceive(x);
                var icParsed = new IcmpPacket(receiveBuffer, bytesRead);

                _onPingReceive(icParsed.IpHeader.SourceAddress);
            }

            if (receiveBuffer != null)
                _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, receiveBuffer);
        }

        //public int GetPingTime(string host)
        //{
        //    if (host == null) return -1;

        //    var serverHe = Dns.GetHostEntry(host);
        //    if (serverHe == null) return -1; // Fehler

        //    // Den IPEndPoint des Servers in einen EndPoint konvertieren.
        //    EndPoint epServer = (new IPEndPoint(serverHe.AddressList[0], 0));

        //    _socket.BeginSendTo(_packet.Buf, 0, _packet.Size, SocketFlags.None, epServer, null, null);

        //    // Initialisiere den Buffer. Der Empfänger-Buffer ist die Größe des
        //    // ICMP Header plus den IP Header (20 bytes)
        //    var receiveBuffer = new byte[256];
        //    _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, x =>
        //    {
        //        var buf = x.AsyncState as byte[];
        //        var ipP = new byte[4];
        //        Array.Copy(buf, 12, ipP, 0, 4);
        //        var dstip = new IPAddress(ipP);
        //        Debugger.Break();
        //    }, receiveBuffer);

        //    //IPAddress.Parse(ReceiveBuffer);
        //    Console.ReadLine();

        //    return 0;
        //}
    }
}