using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UtiLib.Net.Headers;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    public class RawPingDiscovery
    {
        private IcmpPacket _packet;// = new IcmpPacket();
        private Socket _socket;//= new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        private int _intCount = 0;

        private readonly DynamicArray<byte> _payloadData;

        public EventHandler<IcmpPacket> OnResult;

        public RawPingDiscovery()
        {
            _packet = new IcmpPacket();

            _payloadData = _packet.GeneratePayload();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        }

        public void Enqueue(IPAddress input)
        {
            EndPoint epServer = (new IPEndPoint(input, 0));
            _socket.BeginSendTo(_payloadData.Value, 0, _payloadData.Size, SocketFlags.None, epServer, null, null);//SocketFlags.None
            EnsureSocketListen();
        }

        public void Enqueue(IEnumerable<IPAddress> input)
        {
            var ipEndpoints = input.Select(m => new IPEndPoint(m, 0));
            foreach (var ipEndpoint in ipEndpoints)
            {
                _socket.BeginSendTo(_payloadData.Value, 0, _payloadData.Size, SocketFlags.None, ipEndpoint, null, null);//SocketFlags.None
            }
            EnsureSocketListen(20);
        }

        private void EnsureSocketListen(int amount = 1, int listenCap = 20)
        {
            var checkedAmount = Math.Min(amount, listenCap);

            if (_intCount < listenCap)
            {
                for (var i = 0; i < checkedAmount; i++)
                {
                    var receiveBuffer = new byte[256];
                    _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, receiveBuffer);
                }

                Interlocked.Add(ref _intCount, checkedAmount);
            }
        }

        private readonly ConcurrentDictionary<uint, bool> _receivedIps = new ConcurrentDictionary<uint, bool>();

        private void ReceiveCallback(IAsyncResult x)
        {
            var receiveBuffer = x.AsyncState as byte[];
            if (receiveBuffer != null)
            {
                var bytesRead = _socket.EndReceive(x);
                var ipHeader = new IpHeader(receiveBuffer, bytesRead);
                if (ipHeader.ProtocolType == ProtocolType.Icmp)
                {
                    if (!_receivedIps.ContainsKey(ipHeader.RawSourceAddress))
                    {
                        _receivedIps[ipHeader.RawSourceAddress] = true;

                        var icParsed = new IcmpPacket(ipHeader.Data, ipHeader.MessageLength);
                        OnResult?.Invoke(this, icParsed);
                    }
                }
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