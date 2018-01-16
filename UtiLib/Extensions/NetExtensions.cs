using System;
using System.Linq;
using System.Net;

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
    }
}