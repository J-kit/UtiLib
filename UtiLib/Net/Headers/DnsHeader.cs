using System;
using System.IO;
using System.Net;

namespace UtiLib.Net.Headers
{
    public class DnsHeader
    {
        public ushort DestinationPort => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort SourcePort => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort HeaderLength => throw new NotSupportedException("This property is not supported for this header implementation.");

        public short Checksum => throw new NotSupportedException("This property is not supported for this header implementation.");

        public byte[] Data => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort Identification { get; private set; }

        public ushort Flags { get; private set; }

        public ushort TotalQuestions { get; private set; }

        public ushort TotalAnswersRRs { get; private set; }

        public ushort TotalAuthorityRRs { get; private set; }

        public ushort TotalAdditionalRRs { get; private set; }

        public DnsHeader(byte[] dataBuffer, int bytesReceived)
        {
            using (var memoryStream = new MemoryStream(dataBuffer, 0, bytesReceived))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                this.Identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.Flags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalQuestions = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAnswersRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAuthorityRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAdditionalRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            }
        }
    }
}