using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using UtiLib.Net.Headers;
using UtiLib.Reflection;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    /// <summary>
    /// Ping with little to no overhead.
    /// A Bluescreen should not occur.
    /// </summary>
    public class RawPingScan : IPingScaner
    {
        private readonly DynamicArray<byte> _payloadData;

        private readonly ConcurrentDictionary<uint, bool> _receivedIps = new ConcurrentDictionary<uint, bool>();
        private int _intCount;
        private readonly IcmpPacket _packet;

        private readonly Stopwatch _pingStopwatch;
        private readonly Socket _socket;

        public EventHandler<PingCompletedEventArgs> OnResult;
        private IPingScaner _pingScanmerImplementation;

        public RawPingScan()
        {
            _packet = new IcmpPacket();

            _payloadData = _packet.GeneratePayload();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            _pingStopwatch = Stopwatch.StartNew();
        }

        public bool EnableTimeMeasure { get; set; } = true;

        int IPingScaner.MaxConcurrentScans { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IPingScaner.PingCompleted => throw new NotImplementedException();
        TimeSpan IPingScaner.TimeOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        EventHandler IPingScaner.OnPingFinished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        EventHandler<PingCompletedEventArgs> IPingScaner.OnResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Enqueue(IPAddress input)
        {
            var epServer = new IPEndPoint(input, 0);
            SendPing(epServer);
            EnsureSocketListen();
        }

        public void Enqueue(IEnumerable<IPAddress> input)
        {
            var ipEndpoints = input.Select(m => new IPEndPoint(m, 0));
            foreach (var ipEndpoint in ipEndpoints) SendPing(ipEndpoint);
            EnsureSocketListen(20);
        }

        private void SendPing(EndPoint ep)
        {
            DynamicArray<byte> packet;
            if (EnableTimeMeasure)
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

        private void ReceiveCallback(IAsyncResult x)
        {
            var currentTime = _pingStopwatch.ElapsedTicks;
            var elapsedTime = 0;

            var receiveBuffer = x.AsyncState as byte[];
            if (receiveBuffer != null)
            {
                var bytesRead = _socket.EndReceive(x);
                var ipHeader = new IpHeader(receiveBuffer, bytesRead);
                if (ipHeader.ProtocolType == ProtocolType.Icmp)
                    if (!_receivedIps.ContainsKey(ipHeader.RawSourceAddress))
                    {
                        _receivedIps[ipHeader.RawSourceAddress] = true;

                        var icParsed = new IcmpPacket(ipHeader.Data, ipHeader.MessageLength);
                        if (EnableTimeMeasure)
                        {
                            var parsedTime = BitConverter.ToInt64(icParsed.Data, 0);
                            elapsedTime = TimeSpan.FromTicks(currentTime - parsedTime).Milliseconds;
                        }

                        var pingReplyObject = TypeHelper.Construct<PingReply>(receiveBuffer, bytesRead,
                            ipHeader.SourceAddress, elapsedTime);
                        var pingCompletedArgs =
                            TypeHelper.Construct<PingCompletedEventArgs>(pingReplyObject, new Exception(), false, this);

                        OnResult?.Invoke(this, pingCompletedArgs);
                    }
            }

            if (receiveBuffer != null)
                _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback,
                    receiveBuffer);
        }

        void IPingScaner.Enqueue(IEnumerable<IPAddress> addresses)
        {
            throw new NotImplementedException();
        }
    }
}