using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace UtiLib.Net.Headers
{
    public class IpHeader
    {
        private readonly byte _byDifferentiatedServices;

        private readonly byte _byHeaderLength;

        private readonly byte _byProtocol;

        private readonly byte _byTtl;

        private readonly byte _byVersionAndHeaderLength;

        private readonly short _sChecksum;

        private readonly uint _uiDestinationIpAddress;

        private readonly uint _uiSourceIpAddress;

        private readonly ushort _usFlagsAndOffset;

        private readonly ushort _usIdentification;

        private readonly ushort _usTotalLength;

        public IpHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                using (var input = new MemoryStream(byBuffer, 0, nReceived))
                using (var binaryReader = new BinaryReader(input))
                {
                    _byVersionAndHeaderLength = binaryReader.ReadByte();
                    _byDifferentiatedServices = binaryReader.ReadByte();
                    _usTotalLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    _usIdentification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    _usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    _byTtl = binaryReader.ReadByte();
                    _byProtocol = binaryReader.ReadByte();
                    _sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                    _uiSourceIpAddress = (uint)binaryReader.ReadInt32();
                    _uiDestinationIpAddress = (uint)binaryReader.ReadInt32();
                }

                _byHeaderLength = _byVersionAndHeaderLength;
                _byHeaderLength = (byte)(_byHeaderLength << 4);
                _byHeaderLength = (byte)(_byHeaderLength >> 4);
                _byHeaderLength *= 4;

                Data = new byte[_usTotalLength - _byHeaderLength];
                Array.Copy(byBuffer, _byHeaderLength, Data, 0, Data.Length);
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
                var num = _byVersionAndHeaderLength >> 4;
                ProtocolVersion result;
                if (num != 4)
                    result = num != 6 ? ProtocolVersion.Unknown : ProtocolVersion.IPv6;
                else
                    result = ProtocolVersion.IPv4;
                return result;
            }
        }

        public string HeaderLength => _byHeaderLength.ToString();

        public ushort MessageLength => (ushort)(_usTotalLength - _byHeaderLength);

        public string DifferentiatedServices => $"0x{_byDifferentiatedServices:x2} ({_byDifferentiatedServices})";

        public string Flags
        {
            get
            {
                var num = _usFlagsAndOffset >> 13;
                return num == 2 ? "Don't fragment" : (num == 1 ? "More fragments to come" : num.ToString());
            }
        }

        public string FragmentationOffset
        {
            get
            {
                var num = _usFlagsAndOffset << 3;
                return (num >> 3).ToString();
            }
        }

        // ReSharper disable once InconsistentNaming
        public string TTL => _byTtl.ToString();

        public ProtocolType ProtocolType
        {
            get
            {
                if (Enum.IsDefined(typeof(ProtocolType), (int)_byProtocol))
                {
                    return (ProtocolType)_byProtocol;
                }

                return ProtocolType.Unknown;
            }
        }

        public string Checksum => $"0x{_sChecksum:x2}";

        public uint RawSourceAddress => _uiSourceIpAddress;
        public uint RawDestinationAddress => _uiDestinationIpAddress;

        public IPAddress SourceAddress => new IPAddress(_uiSourceIpAddress);

        public IPAddress DestinationAddress => new IPAddress(_uiDestinationIpAddress);

        public string TotalLength => _usTotalLength.ToString();

        public string Identification => _usIdentification.ToString();

        public byte[] Data { get; }
    }
}