using System.Linq;
using System.Net;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class NetExtensions
    {
        public static IPEndPoint AsIpEndPoint(this string address)
        {
            var ipFormat = IPAddress.None;
            int port = default;

            var ipport = address.Split(':');

            if (ipport.Length >= 1)
            {
                ipFormat = ipport[0].AsIpAddress();
            }

            if (ipport.Length >= 2)
            {
                port = Convert.ToInt32(ipport[1]);
            }

            return new IPEndPoint(ipFormat, port);
        }

        public static IPAddress AsIpAddress(this string address)
        {
            return IPAddress.TryParse(address, out var ipAddress) ? ipAddress : Dns.GetHostAddresses(address).FirstOrDefault();
        }

        public static uint ToUint(this IPAddress input)
        {
            Array.Reverse(input.GetAddressBytes());
            return BitConverter.ToUInt32(input.GetAddressBytes(), 0);
        }
    }
}