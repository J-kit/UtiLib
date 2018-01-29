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
        public static PingBase CreatePingEngine(PingEngineCreationFlags flags = PingEngineCreationFlags.Default)
        {
            var rObject = WindowsUser.IsElevated ? new RawPing() : (PingBase)new ApiPing();
            rObject.MeasureTime = flags.HasFlag(PingEngineCreationFlags.MeasureTime);

            if (flags.HasFlag(PingEngineCreationFlags.Subnet))
            {
                rObject.Enqueue(NetMaskHelper.RetrieveSubnetAddresses());
            }

            return rObject;
        }
    }

    [Flags]
    public enum PingEngineCreationFlags
    {
        Default = 1,
        Subnet = 2,
        MeasureTime = 4
    }
}