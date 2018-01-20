using UtiLib.Net.Headers;

namespace UtiLib.Net.Sniffing
{
    public class SniffingPacket
    {
        public IpHeader Header { get; set; }

        public IProtocolHeader Body { get; set; }

        public DnsHeader DnsInfo { get; set; }
    }
}