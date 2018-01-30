using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace UtiLib.Net.Discovery
{
    /// <summary>
    ///     Pings based on <see cref="Ping" /> objects.
    ///     Warning: Aborting an active massping may result in a Bluescreen.
    ///     You may concider using <see cref="RawPing" /> for this purpose
    /// </summary>
    public class ApiPing : PingBase
    {
        private readonly PingOptions _pingOptions;
        private IEnumerator<IPAddress> _currentEnumerator;
        private int _runningPingScanners;

        public ApiPing()
        {
            _pingOptions = new PingOptions(128, true);
        }

        /// <summary>
        ///     An Int32 value that specifies the maximum number of concurrent active scans
        /// </summary>
        public int MaxConcurrentScans { get; set; } = 50;

        /// <inheritdoc />
        /// <summary>
        ///     Starts the ping process.
        ///     Enqueueing new ip addresses for ping is disabled during scantime
        /// </summary>
        public override void Start()
        {
            base.Start();

            var pingLimiter = _runningPingScanners;
            IPAddress lastResult;

            while (pingLimiter <= MaxConcurrentScans && (lastResult = GetNext()) != null)
            {
                var pingSender = new Ping();
                pingSender.PingCompleted += PingCompletedCallback;
                pingSender.SendAsync(lastResult, (int)TimeOut.TotalMilliseconds, new byte[32].Propagate((byte)'#'), _pingOptions);

                pingLimiter++;
            }

            _runningPingScanners = pingLimiter;
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (!e.Reply.Address.Equals(default(IPAddress))
            ) //Happens some times that the ip is 0 // && (e.Reply.Status == IPStatus.Success)
                OnResult?.Invoke(this, e);
            else
                Logger.Log($"Invalid pingreply from pingObject");

            var nextScanAddress = GetNext();
            if (nextScanAddress != null)
                ((Ping)sender).SendAsync(nextScanAddress, (int)TimeOut.TotalMilliseconds,
                    new byte[32].Propagate((byte)'#'), _pingOptions);
            else
                lock (LockObject)
                {
                    _runningPingScanners--;

                    var pingCompleted = _runningPingScanners >= 0 && AddressCollectionQueue.IsEmpty;

                    if (pingCompleted)
                        OnPingFinished?.Invoke(this, new EventArgs());
                }
        }

        private IPAddress GetNext()
        {
            lock (LockObject)
            {
                if (_currentEnumerator != null)
                {
                    if (_currentEnumerator.MoveNext())
                        return _currentEnumerator.Current;

                    _currentEnumerator.Dispose();
                }

                if (AddressCollectionQueue.TryDequeue(out var retVar))
                {
                    _currentEnumerator = retVar.GetEnumerator();
                    return GetNext();
                }

                return null;
            }
        }
    }
}