using System;
using System.IO;
using System.Net;

namespace UtiLib.Net.Headers
{
    public class TcpHeader : IProtocolHeader
    {
        public TcpHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                using (var input = new MemoryStream(byBuffer, 0, nReceived))
                using (var br = new BinaryReader(input))
                {
                    SourcePort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                    DestinationPort = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                    _uiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());
                    _uiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(br.ReadInt32());
                    _usDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                    _usWindow = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                    Checksum = IPAddress.NetworkToHostOrder(br.ReadInt16());
                    _usUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(br.ReadInt16());
                    _byHeaderLength = (byte)(_usDataOffsetAndFlags >> 12);
                    _byHeaderLength *= 4;
                    MessageLength = (ushort)(nReceived - (int)_byHeaderLength);
                    //Array.Copy(byBuffer, (int)_byHeaderLength, Data, 0, nReceived - (int)_byHeaderLength);

                    Data = br.ReadBytes(MessageLength);
                    //var segment = new ArraySegment<byte>(byBuffer, _byHeaderLength, nReceived);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ushort SourcePort { get; }

        public ushort DestinationPort { get; }

        public string SequenceNumber => _uiSequenceNumber.ToString();

        public string AcknowledgementNumber => (_usDataOffsetAndFlags & 16) > 0 ? _uiAcknowledgementNumber.ToString() : "";

        public ushort HeaderLength => _byHeaderLength;

        public string WindowSize => _usWindow.ToString();

        public string UrgentPointer => (_usDataOffsetAndFlags & 32) > 0 ? _usUrgentPointer.ToString() : "";

        //public string Flags
        //{
        //    get
        //    {
        //        int num = (int)(_usDataOffsetAndFlags & 63);
        //        string text = $"0x{num:x2} (";
        //        if ((num & 1) != 0)
        //        {
        //            text += "FIN, ";
        //        }
        //        if ((num & 2) != 0)
        //        {
        //            text += "SYN, ";
        //        }
        //        if ((num & 4) != 0)
        //        {
        //            text += "RST, ";
        //        }
        //        if ((num & 8) != 0)
        //        {
        //            text += "PSH, ";
        //        }
        //        if ((num & 16) != 0)
        //        {
        //            text += "ACK, ";
        //        }
        //        if ((num & 32) != 0)
        //        {
        //            text += "URG";
        //        }
        //        text += ")";
        //        if (text.Contains("()"))
        //        {
        //            text = text.Remove(text.Length - 3);
        //        }
        //        else
        //        {
        //            if (text.Contains(", )"))
        //            {
        //                text = text.Remove(text.Length - 3, 2);
        //            }
        //        }
        //        return text;
        //    }
        //}

        public TcpFlags Flags
        {
            get
            {
                TcpFlags text = 0;

                var num = (int)(_usDataOffsetAndFlags & 63);

                if ((num & 1) != 0)
                    text |= TcpFlags.Fin;

                if ((num & 2) != 0)
                    text |= TcpFlags.Syn;

                if ((num & 4) != 0)
                    text |= TcpFlags.Rst;

                if ((num & 8) != 0)
                    text |= TcpFlags.Psh;

                if ((num & 16) != 0)
                    text |= TcpFlags.Ack;

                if ((num & 32) != 0)
                    text |= TcpFlags.Urg;

                return text;
            }
        }

        public short Checksum { get; } = 555;

        public byte[] Data { get; } //= new byte[4096];

        public ushort MessageLength { get; }

        private readonly uint _uiSequenceNumber = 555u;

        private readonly uint _uiAcknowledgementNumber = 555u;

        private readonly ushort _usDataOffsetAndFlags = 555;

        private readonly ushort _usWindow = 555;

        private readonly ushort _usUrgentPointer;

        private readonly byte _byHeaderLength;
    }
}