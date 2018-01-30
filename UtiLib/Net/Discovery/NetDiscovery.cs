using System;
using UtiLib.Environment;
using UtiLib.Net.Headers;

namespace UtiLib.Net.Discovery
{
    public static class NetDiscovery
    {
        /// <summary>
        /// Returns Ping discovery engine based on the current users' permissons
        /// </summary>
        /// <returns></returns>
        public static PingBase CreatePingEngine(PingEngineFlags flags = PingEngineFlags.Default)
        {
            return (WindowsUser.IsElevated ? new RawPing() : (PingBase)new ApiPing()).Prepare(flags);
        }

        public static T Prepare<T>(this T input, PingEngineFlags flags) where T : PingBase
        {
            input.MeasureTime = flags.HasFlag(PingEngineFlags.MeasureTime);

            if (flags.HasFlag(PingEngineFlags.Subnet))
            {
                input.Enqueue(NetMaskHelper.RetrieveSubnetAddresses());
            }
            return input;
        }
    }

    [Flags]
    public enum PingEngineFlags
    {
        Default = 1,

        /// <summary>
        /// Scans subnet addresses of all interfaces
        /// </summary>
        Subnet = 2,

        /// <summary>
        /// Enable Time measurement
        /// </summary>
        MeasureTime = 4
    }
}