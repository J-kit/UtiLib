using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UtiLib.Net.Headers;
using UtiLib.Reflection;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    public class RawPing : PingBase
    {
        private readonly DynamicArray<byte> _payloadData;
        private readonly Stopwatch _pingStopwatch;
        private readonly ConcurrentDictionary<uint, bool> _receivedIps = new ConcurrentDictionary<uint, bool>();
        private DateTime _lastResult = DateTime.Now;

        private Socket _socket;
        private Timer _timeoutTimer;

        public RawPing()
        {
            _pingStopwatch = Stopwatch.StartNew();
            _payloadData = IcmpPacket.GenerateNew();
        }

        ~RawPing()
        {
            Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Starts the ping process.
        ///     Enqueueing new ip addresses for ping is disabled during scantime
        /// </summary>
        public override void Start()
        {
            base.Start();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            StartListen(20);
            lock (LockObject)
            {
                while (AddressCollectionQueue.TryDequeue(out var currentAddressSet))
                    foreach (var ipEndPoint in currentAddressSet.Select(m => new IPEndPoint(m, 0)))
                        SendPing(ipEndPoint);

                UpdateLastResult();
            }

            if (TimeOut < TimeSpan.FromMinutes(10))
                _timeoutTimer = new Timer(PingTimeoutCheckerCallback, this, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
        }

        //TODO
        public override async Task StartAsync()
        {
            throw new NotImplementedException();
        }

        private void PingTimeoutCheckerCallback(object state)
        {
            lock (LockObject)
            {
                if (AddressCollectionQueue.IsEmpty && TimeOut < DateTime.Now.Subtract(_lastResult))
                {
                    _timeoutTimer.Dispose();
                    OnPingFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void SendPing(EndPoint ep)
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

        private void StartListen(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var receiveBuffer = new byte[256];
                _socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback,
                    receiveBuffer);
            }
        }

        private void UpdateLastResult()
        {
            lock (LockObject)
            {
                _lastResult = DateTime.Now;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var currentTime = _pingStopwatch.ElapsedTicks;
            var elapsedTime = 0;

            var bytesRead = _socket.EndReceive(ar);
            if (ar.AsyncState is byte[] buffer)
            {
                var ipHeader = new IpHeader(buffer, bytesRead);
                if (ipHeader.ProtocolType == ProtocolType.Icmp && !_receivedIps.ContainsKey(ipHeader.RawSourceAddress))
                {
                    UpdateLastResult();
                    _receivedIps[ipHeader.RawSourceAddress] = true;

                    var icParsed = new IcmpPacket(ipHeader.Data, ipHeader.MessageLength);

                    if (MeasureTime)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        var parsedTime = BitConverter.ToInt64(icParsed.Data.Array, icParsed.Data.Offset);
                        elapsedTime = TimeSpan.FromTicks(currentTime - parsedTime).Milliseconds;
                    }

                    var pingReplyObject =
                        TypeHelper.Construct<PingReply>(buffer, bytesRead, ipHeader.SourceAddress, elapsedTime);
                    var pingCompletedArgs =
                        TypeHelper.Construct<PingCompletedEventArgs>(pingReplyObject, new Exception(), false, this);

                    OnResult?.Invoke(this, pingCompletedArgs);
                }
            }
        }

        public override void Dispose()
        {
            lock (LockObject)
            {
                if (!_disposed)
                {
                    _socket?.Dispose();
                    _timeoutTimer.Dispose();
                    base.Dispose();
                }
            }
        }
    }
}