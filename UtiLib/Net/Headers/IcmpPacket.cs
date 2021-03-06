﻿using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using UtiLib.Shared.Generic;

namespace UtiLib.Net.Headers
{
    /// <summary>
    /// Generates or parses ICMP requests/responses
    /// </summary>
    public class IcmpPacket : IProtocolHeader
    {
        private const int DataPtr = 8;

        #region IProtocolHeader Implementation

        ushort IProtocolHeader.DestinationPort { get; } = 0;
        ushort IProtocolHeader.SourcePort { get; } = 0;
        ushort IProtocolHeader.HeaderLength => (ushort)PingData;
        short IProtocolHeader.Checksum => (short)CheckSum;

        #endregion IProtocolHeader Implementation

        public IPStatus IpStatus => (IPStatus)Type;

        /// <summary>
        ///     Message type aka. <see cref="IpStatus"/>
        /// </summary>
        public byte Type { get; set; }              // Message Typ

        /// <summary>
        ///     Subcode type
        /// </summary>
        public byte SubCode { get; set; }           // Subcode Typ

        /// <summary>
        ///     Packet checksum
        /// </summary>
        public ushort CheckSum { get; set; }        // Checksumme

        /// <summary>
        ///     Packet identifier
        /// </summary>
        public ushort Identifier { get; set; }      // Identifizierer

        /// <summary>
        /// Packet Sequencenumber
        /// </summary>
        public ushort SequenceNumber { get; set; }  // Sequenznummer

        public ArraySegment<byte> Data { get; set; }            // Byte Array

        public int PingData { get; set; }

        private const int IcmpEcho = 8;

        /// <summary>
        /// Parses the results of <see cref="IpHeader"/>
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <param name="bytesReceived"></param>
        public IcmpPacket(byte[] dataBuffer, int bytesReceived)
        {
            using (var ms = new MemoryStream(dataBuffer, 0, bytesReceived))
            using (var br = new BinaryReader(ms))
            {
                Type = br.ReadByte();
                SubCode = br.ReadByte();

                CheckSum = br.ReadUInt16();
                Identifier = br.ReadUInt16();
                SequenceNumber = br.ReadUInt16();

                var messageLength = (int)(bytesReceived - DataPtr);
                if (messageLength > 0)
                {
                    Data = new ArraySegment<byte>(dataBuffer, DataPtr, messageLength);
                }
            }
        }

        /// <summary>
        ///     Initializes packet for generation
        /// </summary>
        /// <param name="data"></param>
        public IcmpPacket(byte[] data = null)
        {
            Data = data == null ? default : new ArraySegment<byte>(data);
            Reset();
        }

        /// <summary>
        ///     Serializes the current object to a <see cref="DynamicArray"/> which is ready to send over raw sockets
        /// </summary>
        /// <returns></returns>
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

            CheckSum = 0;

            return DynamicArray.Create(sendbuf, packetSize);
        }

        private bool CalculateChecksum(int packetSize)
        {
            var icmpHeaderBufferIndex = 0;

            var icmpPktBuffer = new byte[packetSize];
            var index = Serialize(icmpPktBuffer, packetSize, PingData);

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
            WriteArray(Data.ToArray(), buffer, ref index, pingData);

            if (index != packetSize) /* sizeof(IcmpPacket) */
            {
                return R<int>.Err("Invalid PacketSize");
            }

            return index;
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
            if (Data == default)
            {
                Data = new ArraySegment<byte>(new byte[PingData].Propagate((byte)'#'));
            }
            else
            {
                PingData = Data.Count;
            }
        }

        private static void WriteArray(Array sourceArray, Array destinationArray, ref int index, int? length = default)
        {
            var rl = length ?? sourceArray.Length;
            Array.Copy(sourceArray, 0, destinationArray, index, rl);
            index += rl;
        }

        private static ushort Checksum(ushort[] buffer, int size)
        {
            var cksum = 0;
            var counter = 0;

            for (var s = size - 1; s >= 0; s--)
            {
                cksum += buffer[counter];
                counter += 1;
            }

            cksum = (cksum >> 16) + (cksum & 0xffff);
            cksum += (cksum >> 16);
            return (ushort)(~cksum);
        }

        public static DynamicArray<byte> GenerateNew(byte[] data = null)
        {
            return new IcmpPacket(data).GeneratePayload();
        }
    }
}