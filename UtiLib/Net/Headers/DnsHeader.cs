using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace UtiLib.Net.Headers
{
    public class DnsHeader
    {
        private const int DataPtr = 12;

        public ushort DestinationPort => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort SourcePort => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort HeaderLength => throw new NotSupportedException("This property is not supported for this header implementation.");

        public short Checksum => throw new NotSupportedException("This property is not supported for this header implementation.");

        //  public byte[] Data => throw new NotSupportedException("This property is not supported for this header implementation.");

        public ushort Identification { get; private set; }

        public ushort Flags { get; private set; }

        public ushort TotalQuestions { get; private set; }

        public ushort TotalAnswersRRs { get; private set; }

        public ushort TotalAuthorityRRs { get; private set; }

        public ushort TotalAdditionalRRs { get; private set; }

        public ArraySegment<byte> Data { get; }

        public DnsHeader(ArraySegment<byte> dataBuffer)
        {
            using (var memoryStream = new MemoryStream(dataBuffer.Array ?? throw new InvalidOperationException(), dataBuffer.Offset, dataBuffer.Count))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                this.Identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.Flags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalQuestions = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAnswersRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAuthorityRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TotalAdditionalRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                //var res = Settings.DefaultEncoding.GetString(dataBuffer, (int)memoryStream.Position, (int)(dataBuffer.Length - memoryStream.Position));
                var cDataPtr = dataBuffer.Offset + DataPtr;
                var messageLength = (int)(dataBuffer.Count - cDataPtr);
                if (messageLength > 0)
                {
                    Data = new ArraySegment<byte>(dataBuffer.Array, cDataPtr, messageLength);
                }
            }
        }
    }

    /*
    [Marshalling.Endian(Marshalling.Endianness.Big)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header
    {
        public const int SIZE = 12;

        public static Header FromArray(byte[] header)
        {
            if (header.Length < SIZE)
            {
                throw new ArgumentException("Header length too small");
            }

            return Marshalling.Struct.GetStruct<Header>(header, 0, SIZE);
        }

        private ushort id;

        private byte flag0;
        private byte flag1;

        // Question count: number of questions in the Question section
        private ushort qdCount;

        // Answer record count: number of records in the Answer section
        private ushort anCount;

        // Authority record count: number of records in the Authority section
        private ushort nsCount;

        // Additional record count: number of records in the Additional section
        private ushort arCount;

        public int Id
        {
            get { return id; }
            set { id = (ushort)value; }
        }

        public int QuestionCount
        {
            get { return qdCount; }
            set { qdCount = (ushort)value; }
        }

        public int AnswerRecordCount
        {
            get { return anCount; }
            set { anCount = (ushort)value; }
        }

        public int AuthorityRecordCount
        {
            get { return nsCount; }
            set { nsCount = (ushort)value; }
        }

        public int AdditionalRecordCount
        {
            get { return arCount; }
            set { arCount = (ushort)value; }
        }

        public bool Response
        {
            get { return Qr == 1; }
            set { Qr = Convert.ToByte(value); }
        }

        public OperationCode OperationCode
        {
            get { return (OperationCode)Opcode; }
            set { Opcode = (byte)value; }
        }

        public bool AuthorativeServer
        {
            get { return Aa == 1; }
            set { Aa = Convert.ToByte(value); }
        }

        public bool Truncated
        {
            get { return Tc == 1; }
            set { Tc = Convert.ToByte(value); }
        }

        public bool RecursionDesired
        {
            get { return Rd == 1; }
            set { Rd = Convert.ToByte(value); }
        }

        public bool RecursionAvailable
        {
            get { return Ra == 1; }
            set { Ra = Convert.ToByte(value); }
        }

        public ResponseCode ResponseCode
        {
            get { return (ResponseCode)RCode; }
            set { RCode = (byte)value; }
        }

        public int Size
        {
            get { return Header.SIZE; }
        }

        public byte[] ToArray()
        {
            return Marshalling.Struct.GetBytes(this);
        }

        public override string ToString()
        {
            return ObjectStringifier.New(this)
                .AddAll()
                .Remove("Size")
                .ToString();
        }

        // Query/Response Flag
        private byte Qr
        {
            get { return Flag0.GetBitValueAt(7, 1); }
            set { Flag0 = Flag0.SetBitValueAt(7, 1, value); }
        }

        // Operation Code
        private byte Opcode
        {
            get { return Flag0.GetBitValueAt(3, 4); }
            set { Flag0 = Flag0.SetBitValueAt(3, 4, value); }
        }

        // Authorative Answer Flag
        private byte Aa
        {
            get { return Flag0.GetBitValueAt(2, 1); }
            set { Flag0 = Flag0.SetBitValueAt(2, 1, value); }
        }

        // Truncation Flag
        private byte Tc
        {
            get { return Flag0.GetBitValueAt(1, 1); }
            set { Flag0 = Flag0.SetBitValueAt(1, 1, value); }
        }

        // Recursion Desired
        private byte Rd
        {
            get { return Flag0.GetBitValueAt(0, 1); }
            set { Flag0 = Flag0.SetBitValueAt(0, 1, value); }
        }

        // Recursion Available
        private byte Ra
        {
            get { return Flag1.GetBitValueAt(7, 1); }
            set { Flag1 = Flag1.SetBitValueAt(7, 1, value); }
        }

        // Zero (Reserved)
        private byte Z
        {
            get { return Flag1.GetBitValueAt(4, 3); }
            set { }
        }

        // Response Code
        private byte RCode
        {
            get { return Flag1.GetBitValueAt(0, 4); }
            set { Flag1 = Flag1.SetBitValueAt(0, 4, value); }
        }

        private byte Flag0
        {
            get { return flag0; }
            set { flag0 = value; }
        }

        private byte Flag1
        {
            get { return flag1; }
            set { flag1 = value; }
        }
    }*/
}