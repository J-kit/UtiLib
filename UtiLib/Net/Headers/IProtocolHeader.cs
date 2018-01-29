using System;

namespace UtiLib.Net.Headers
{
    public interface IProtocolHeader
    {
        ushort DestinationPort { get; }

        ushort SourcePort { get; }

        ushort HeaderLength { get; }

        short Checksum { get; }

        ArraySegment<byte> Data { get; }
    }
}