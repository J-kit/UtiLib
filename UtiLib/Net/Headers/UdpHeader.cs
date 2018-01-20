using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace UtiLib.Net.Headers
{
    public class UdpHeader : IProtocolHeader
    {
        public ushort DestinationPort { get; private set; }

        public ushort SourcePort { get; private set; }

        public ushort HeaderLength { get; private set; }

        public short Checksum { get; private set; }

        public byte[] Data { get; private set; }

        public UdpHeader(byte[] dataBuffer, int bytesReceived)
        {
            if (dataBuffer == null || bytesReceived == 0)
            {
                throw new InvalidDataException("The parameters can't be null / zero.");
            }
            using (var memoryStream = new MemoryStream(dataBuffer, 0, bytesReceived))
            using (var br = new BinaryReader(memoryStream))
            {
                SourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                DestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                HeaderLength = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                Checksum = IPAddress.NetworkToHostOrder(br.ReadInt16());
                //Data = new byte[bytesReceived - 8];
                //Array.Copy(dataBuffer, 8, Data, 0, bytesReceived - 8);

                Data = br.ReadBytes((int)(bytesReceived - br.BaseStream.Position));
                //    Debugger.Break();
            }
        }
    }
}