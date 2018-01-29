using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace UtiLib.Net.Discovery
{
    public class NetInterfaceInfos
    {
        public string Name { get; set; }
        public IPAddress Address { get; set; }
    }

    public class NetMaskHelper
    {
        //+		IPv4Mask	{255.255.255.0}	System.Net.IPAddress
        //+		Address	    {10.0.0.6}	System.Net.IPAddress

        /// <summary>
        /// Returns all Addresses of all Adapters' subnets
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IPAddress> RetrieveSubnetAddresses()
        {
            //Gather all networkinterface information required
            var interfaceInfos = NetworkInterface.GetAllNetworkInterfaces()
                .Where(m => m.NetworkInterfaceType != NetworkInterfaceType.Loopback && m.OperationalStatus == OperationalStatus.Up)
                .SelectMany(m => m.GetIPProperties().UnicastAddresses)
                .Where(m => m.Address.AddressFamily == AddressFamily.InterNetwork).SelectMany(m => GetIpSubnet(m.Address, m.IPv4Mask));

            return interfaceInfos;
        }

        //+		IPv4Mask	{255.255.255.0}	System.Net.IPAddress
        //+		Address	    {10.0.0.6}	System.Net.IPAddress

        /// <summary>
        /// Get all IPAdresses which are in the current subnet
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<NetInterfaceInfos> GetInterfaces()
        {
            //Gather all networkinterface information required
            var interfaceInfos = NetworkInterface.GetAllNetworkInterfaces()
                .Where(m => m.NetworkInterfaceType != NetworkInterfaceType.Loopback && m.OperationalStatus == OperationalStatus.Up)
                .Where(m => m.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || m.NetworkInterfaceType == NetworkInterfaceType.Ethernet);

            foreach (var ni in interfaceInfos)
            {
                foreach (var ip in ni.GetIPProperties().UnicastAddresses.Select(m => m.Address))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        yield return new NetInterfaceInfos { Name = ni.Name, Address = ip };
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all ips between the two ones
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        public static IEnumerable<IPAddress> GetIpSubnet(IPAddress ip, IPAddress ipmask)
        {
            var maskBytes = ipmask.GetAddressBytes();
            var unaryId = BitConverter.ToUInt32(ip.GetAddressBytes(), 0) & BitConverter.ToUInt32(maskBytes, 0);
            var netId = BitConverter.GetBytes(unaryId);
            var startIp = BitConverter.GetBytes(BitConverter.ToUInt32(netId, 0) ^ BitConverter.ToUInt32(maskBytes.ToArray(r => (byte)(byte.MaxValue - r)), 0));

            foreach (var cIp in GetIpRange(netId, startIp))
            {
                yield return cIp;
            }
        }

        /// <summary>
        /// Calculate all ips between the two ones
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        private static IEnumerable<IPAddress> GetIpRange(byte[] ip1, byte[] ip2)
        {
            var bMax = BitConverter.ToUInt32(ip2.Reverse().ToArray(), 0) - 1;

            for (var n = BitConverter.ToUInt32(ip1.Reverse().ToArray(), 0) + 1; n <= bMax; n++)
            {
                yield return new IPAddress(BitConverter.GetBytes(n).Reverse().ToArray());
            }

            yield break;
        }
    }
}