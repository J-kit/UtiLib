using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using UtiLib.Net.Headers;

namespace UtiLib.Net.Discovery
{
    internal class NetDiscovery
    {
    }

    public class PingScan
    {
        private readonly PingOptions _pingOptions;

        private readonly int _pingTimeout;

        private int _runningPingScanners;

        /// <summary>
        ///     Null means that the process has been finished
        /// </summary>
        private readonly Action<IPAddress> _scanSuccessCallback;

        private readonly ConcurrentQueue<IPAddress> AddressQueue = new ConcurrentQueue<IPAddress>();

        public PingScan(Action<IPAddress> scanSuccessCallback, int timeout = 10000)
        {
            _pingOptions = new PingOptions(128, true);
            _scanSuccessCallback = scanSuccessCallback;
            _pingTimeout = timeout;
        }

        public bool PingCompleted { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="uMlimiter">Prevents memory leakage</param>
        public void InitScan(IPAddress[] addresses, int uMlimiter = 1000)
        {
            var uClimiter = 0;
            //Init Ping scan

            foreach (var address in addresses)
            {
                if (uClimiter >= uMlimiter)
                {
                    AddressQueue.Enqueue(address);
                }
                else
                {
                    uClimiter++;
                    _runningPingScanners++;
                    var pingSender = new Ping();
                    pingSender.PingCompleted += PingCompletedCallback;
                    pingSender.SendAsync(address, _pingTimeout, Encoding.ASCII.GetBytes("a"), _pingOptions);
                }
            }
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (!e.Reply.Address.Equals(default(IPAddress))) //Happens some times that the ip is 0
            {
                if (e.Reply.Status == IPStatus.Success)
                {
                    //Enqueue PortScan
                    _scanSuccessCallback?.Invoke(e.Reply.Address);
                }
            }

            if (!AddressQueue.TryDequeue(out var nextScanAddress))
            {
                return;
            }

            if (nextScanAddress != null)
            {
                ((Ping)sender).SendAsync(nextScanAddress, _pingTimeout, Encoding.ASCII.GetBytes("a"), _pingOptions);
            }
            else
            {
                Interlocked.Decrement(ref _runningPingScanners);
                PingCompleted = _runningPingScanners == 0 && AddressQueue.Count == 0;
                if (PingCompleted) _scanSuccessCallback?.Invoke(null);
            }
        }
    }
}