using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using UtiLib.IO;
using UtiLib.Net.Headers;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Discovery
{
    public class IcmpPacket
    {
        public IpHeader IpHeader { get; set; }

        public byte Type { get; set; }              // Message Typ
        public byte SubCode { get; set; }           // Subcode Typ

        public ushort CheckSum { get; set; }        // Checksumme
        public ushort Identifier { get; set; }      // Identifizierer
        public ushort SequenceNumber { get; set; }  // Sequenznummer
        public byte[] Data { get; set; }            // Byte Array

        public int PingData { get; set; }

        private const int SocketError = -1;
        private const int IcmpEcho = 8;

        public IcmpPacket(byte[] rawBytes, int nReceived)
        {
            LoadBuffer(rawBytes, nReceived);
        }

        public IcmpPacket()
        {
            Reset();
        }

        /// <summary>
        /// Loads rawpackets
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <param name="nReceived"></param>
        public void LoadBuffer(byte[] rawBytes, int nReceived)
        {
            IpHeader = new IpHeader(rawBytes, nReceived);
            Console.WriteLine(IpHeader.ProtocolType);
            using (var input = new MemoryStream(IpHeader.Data))
            using (var br = new BinaryReader(input))
            {
                Type = br.ReadByte();
                SubCode = br.ReadByte();

                CheckSum = br.ReadUInt16();
                Identifier = br.ReadUInt16();
                SequenceNumber = br.ReadUInt16();
                Data = br.ReadBytes((int)(IpHeader.Data.Length - input.Position));
                Debugger.Break();
            }
        }

        public DynamicArray<byte> GeneratePayload()
        {
            var packetSize = PingData + 8;
            if (packetSize % 2 == 1) ++packetSize;
            var sendbuf = new byte[packetSize];

            if (!CalculateChecksum(packetSize))
            {
                throw new InvalidDataException("Faulty checksum Calculation");
            }

            if (!Serialize(sendbuf, packetSize, PingData).Ok)
                throw new InvalidDataException("Data serialisation failed");

            Console.WriteLine(CheckSum);
            Console.WriteLine(BitConverter.GetBytes(CheckSum).GetString(StringEncoding.FormattedByte));

            CheckSum = 0;

            return DynamicArray.Create(sendbuf, packetSize);
        }

        private bool CalculateChecksum(int packetSize)
        {
            var icmpHeaderBufferIndex = 0;

            var icmpPktBuffer = new byte[packetSize];
            var index = Serialize(icmpPktBuffer, packetSize, PingData);

            // Console.WriteLine($"{PingData} / {packetSize} / {icmpPktBuffer.ToHash<MD5>().GetHexString()}");

            if (!index.Ok)
                return false;

            var cksumBufferLength = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(index.Value) / 2));
            var cksumBuffer = new ushort[cksumBufferLength];

            for (var i = 0; i < cksumBufferLength; i++)
            {
                cksumBuffer[i] = BitConverter.ToUInt16(icmpPktBuffer, icmpHeaderBufferIndex);
                icmpHeaderBufferIndex += 2;
            }

            CheckSum = Checksum(cksumBuffer, cksumBufferLength);
            return true;
        }

        private R<int> Serialize(byte[] buffer, int packetSize, int pingData)
        {
            var index = 0;

            var bCksum = BitConverter.GetBytes(CheckSum);
            var bId = BitConverter.GetBytes(Identifier);
            var bSeq = BitConverter.GetBytes(SequenceNumber);

            buffer[0] = Type;
            buffer[1] = SubCode;
            index = 2;

            WriteArray(bCksum, buffer, ref index);
            WriteArray(bId, buffer, ref index);
            WriteArray(bSeq, buffer, ref index);
            WriteArray(Data, buffer, ref index, pingData);

            if (index != packetSize) /* sizeof(IcmpPacket) */
            {
                return R<int>.Err("Invalid PacketSize");
            }

            return index;
        }

        private void WriteArray(Array sourceArray, Array destinationArray, ref int index, int? length = default)
        {
            var rl = length ?? sourceArray.Length;
            Array.Copy(sourceArray, 0, destinationArray, index, rl);
            index += rl;
        }

        private ushort Checksum(ushort[] buffer, int size)
        {
            var cksum = 0;
            var counter = 0;

            while (size > 0)
            {
                cksum += Convert.ToInt32(buffer[counter]);
                counter += 1;
                size -= 1;
            }

            cksum = (cksum >> 16) + (cksum & 0xffff);
            cksum += (cksum >> 16);
            return (ushort)(~cksum);
        }

        /// <summary>
        /// Resets the data of the packet
        /// </summary>
        public void Reset()
        {
            Type = IcmpEcho;
            SubCode = 0;
            CheckSum = 0;
            Identifier = 45;
            SequenceNumber = 500;
            PingData = 32;

            Data = new byte[PingData].Propagate((byte)'#');
        }
    }

    //public class IcmpPacket1 : IDisposable
    //{
    //    public readonly byte Type;               // Message Typ
    //    public readonly byte SubCode;            // Subcode Typ
    //    public readonly byte[] Data;             // Byte Array
    //    public ushort CheckSum;                  // Checksumme
    //    public readonly ushort Identifier;       // Identifizierer
    //    public readonly ushort SequenceNumber;   // Sequenznummer

    //    public int PingData { get; set; }

    //    private const int SocketError = -1;
    //    private const int IcmpEcho = 8;

    //    public IcmpPacket1()
    //    {
    //        Type = IcmpEcho;
    //        SubCode = 0;
    //        CheckSum = 0;
    //        Identifier = 45;
    //        SequenceNumber = 500;
    //        PingData = 32;

    //        Data = new byte[PingData].Propagate((byte)'#');
    //    }

    //    public void Dispose()
    //    {
    //        //_buffer?.Dispose();
    //    }
    //}
}