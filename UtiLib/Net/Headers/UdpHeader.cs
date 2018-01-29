using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace UtiLib.Net.Headers
{
    public class UdpHeader : IProtocolHeader
    {
        private const int DataPtr = 8;

        public ushort DestinationPort { get; private set; }

        public ushort SourcePort { get; private set; }

        public ushort HeaderLength { get; private set; }

        public short Checksum { get; private set; }

        public ArraySegment<byte> Data { get; private set; }

        public UdpHeader(byte[] dataBuffer, int nReceived)
        {
            if (dataBuffer == null || nReceived == 0)
            {
                throw new InvalidDataException("The parameters can't be null / zero.");
            }
            using (var ms = new MemoryStream(dataBuffer, 0, nReceived))
            using (var br = new BinaryReader(ms))
            {
                SourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                DestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                HeaderLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                Checksum = IPAddress.NetworkToHostOrder(br.ReadInt16());

                var messageLength = (ushort)(nReceived - DataPtr);
                if (messageLength > 0)
                {
                    Data = new ArraySegment<byte>(dataBuffer, DataPtr, messageLength);
                }
            }
        }
    }
}