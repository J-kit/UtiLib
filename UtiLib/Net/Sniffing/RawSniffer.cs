using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Net.Headers;
using UtiLib.Shared.Enums;

namespace UtiLib.Net.Sniffing
{
    public class RawSniffer
    {
        public IPAddress NetworkAdapter { get; set; }

        public ushort BufferSize { get; set; } = 4096;

        private Predicate<SniffingPacket> _pred;

        private Socket _sniffingSocket;

        public Predicate<SniffingPacket> FilteringRule
        {
            set => _pred = value;
        }

        public event EventHandler<IpHeader> OnUnknownPacketCaptured;

        public event EventHandler<SniffingPacket> OnPacketCaptured;

        public void Stop()
        {
            if (_sniffingSocket != null)
            {
                _sniffingSocket.Close();
                _sniffingSocket.Dispose();
            }
        }

        public void Start()
        {
            var array = new byte[] { 1, 0, 0, 0 };
            var array2 = new byte[] { 1, 0, 0, 0 };

            _sniffingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

            _sniffingSocket.Bind(new IPEndPoint(NetworkAdapter, 0));
            _sniffingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AcceptConnection, true);

            _sniffingSocket.IOControl(IOControlCode.ReceiveAll, array, array2);
            for (int i = 0; i < 20; i++)
            {
                var buffer = new byte[BufferSize];
                _sniffingSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.DataReadCallback, buffer);
            }
        }

        private void DataReadCallback(IAsyncResult ar)
        {
            var buffer = ar.AsyncState as byte[];
            try
            {
                var num = _sniffingSocket.EndReceive(ar);
                if (buffer != null)
                {
                    if (num > 0)
                    {
                        ProcessData(buffer, num);
                    }
                    _sniffingSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, this.DataReadCallback, buffer);
                }
                else
                {
                    Logger.Log($"Error: AsyncState != byte[]", LogSeverity.Error);
                }
            }
            catch (Exception ex)
            {
                if (ex is SocketException sex)
                {
                    if (sex.NativeErrorCode == 10040)
                    {
                        buffer = buffer ?? new byte[BufferSize];
                        _sniffingSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, DataReadCallback, null);
                    }
                    else
                    {
                        Logger.Log(ex);
                        throw;
                    }
                }
                else
                {
                    Logger.Log(ex);
                    throw;
                }
            }
        }

        private void ProcessData(byte[] dataBuffer, int bytesReceived)
        {
            var ipHeader = new IpHeader(dataBuffer, bytesReceived);
            var packet = new SniffingPacket { Header = ipHeader };

            switch (ipHeader.ProtocolType)
            {
                case ProtocolType.Tcp:
                    packet.Body = new TcpHeader(ipHeader.Data, ipHeader.MessageLength);
                    break;

                case ProtocolType.Udp:
                    packet.Body = new UdpHeader(ipHeader.Data, ipHeader.MessageLength);
                    break;

                case ProtocolType.Icmp:
                    packet.Body = new IcmpPacket(ipHeader.Data, ipHeader.MessageLength);
                    break;

                default:
                    OnUnknownPacketCaptured?.Invoke(this, ipHeader);
                    return;
            }

            if (53.MultiEqualsOr(packet.Body.SourcePort, packet.Body.DestinationPort))
            {
                packet.DnsInfo = new DnsHeader(ipHeader.Data, (int)ipHeader.MessageLength);
            }

            if (_pred?.Invoke(packet) ?? true)
            {
                this.OnPacketCaptured?.Invoke(this, packet);
            }
        }
    }
}