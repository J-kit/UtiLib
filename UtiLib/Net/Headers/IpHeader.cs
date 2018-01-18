using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace UtiLib.Net.Headers
{
    public class IpHeader
    {
        private readonly byte byDifferentiatedServices;

        private readonly byte byHeaderLength;

        private readonly byte byProtocol;

        private readonly byte byTTL;

        private readonly byte byVersionAndHeaderLength;

        private readonly short sChecksum;

        private readonly uint uiDestinationIPAddress;

        private readonly uint uiSourceIPAddress;

        private readonly ushort usFlagsAndOffset;

        private readonly ushort usIdentification;

        private readonly ushort usTotalLength;

        public IpHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                using (var input = new MemoryStream(byBuffer, 0, nReceived))
                using (var binaryReader = new BinaryReader(input))
                {
                    byVersionAndHeaderLength = binaryReader.ReadByte();
                    byDifferentiatedServices = binaryReader.ReadByte();
                    usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    byTTL = binaryReader.ReadByte();
                    byProtocol = binaryReader.ReadByte();
                    sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    uiSourceIPAddress = (uint)binaryReader.ReadInt32();
                    uiDestinationIPAddress = (uint)binaryReader.ReadInt32();
                }

                byHeaderLength = byVersionAndHeaderLength;
                byHeaderLength = (byte)(byHeaderLength << 4);
                byHeaderLength = (byte)(byHeaderLength >> 4);
                byHeaderLength *= 4;

                Data = new byte[usTotalLength - byHeaderLength];
                Array.Copy(byBuffer, byHeaderLength, Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ProtocolVersion Version
        {
            get
            {
                var num = byVersionAndHeaderLength >> 4;
                ProtocolVersion result;
                if (num != 4)
                    result = num != 6 ? ProtocolVersion.Unknown : ProtocolVersion.IPv6;
                else
                    result = ProtocolVersion.IPv4;
                return result;
            }
        }

        public string HeaderLength => byHeaderLength.ToString();

        public ushort MessageLength => (ushort)(usTotalLength - byHeaderLength);

        public string DifferentiatedServices => $"0x{byDifferentiatedServices:x2} ({byDifferentiatedServices})";

        public string Flags
        {
            get
            {
                var num = usFlagsAndOffset >> 13;
                return num == 2 ? "Don't fragment" : (num == 1 ? "More fragments to come" : num.ToString());
            }
        }

        public string FragmentationOffset
        {
            get
            {
                var num = usFlagsAndOffset << 3;
                return (num >> 3).ToString();
            }
        }

        // ReSharper disable once InconsistentNaming
        public string TTL => byTTL.ToString();

        public ProtocolType ProtocolType
        {
            get
            {
                if (Enum.IsDefined(typeof(ProtocolType), byProtocol))
                {
                    return (ProtocolType)byProtocol;
                }

                return ProtocolType.Unknown;
            }
        }

        public string Checksum => $"0x{sChecksum:x2}";

        public IPAddress SourceAddress => new IPAddress(uiSourceIPAddress);

        public IPAddress DestinationAddress => new IPAddress(uiDestinationIPAddress);

        public string TotalLength => usTotalLength.ToString();

        public string Identification => usIdentification.ToString();

        public byte[] Data { get; }
    }
}