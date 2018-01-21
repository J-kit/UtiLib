using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace UtiLib.Net.Discovery
{
    /// <summary>
    /// Pings based on <see cref="Ping"/> objects.
    /// Warning: Aborting an active massping may result in a Bluescreen.
    /// You may concider using <see cref="RawPingScan"/> for this purpose
    /// </summary>
    public class PingScan : IPingScaner
    {
        private readonly Queue<IEnumerator<IPAddress>> _ipQueue = new Queue<IEnumerator<IPAddress>>();

        private readonly object _lockObject = new object();
        private readonly PingOptions _pingOptions;
        private IEnumerator<IPAddress> _currentEnumerator;
        private int _runningPingScanners;

        /// <summary>
        ///     Will be called when each ping has been completed
        /// </summary>
        public EventHandler OnPingFinished { get; set; }

        /// <summary>
        ///     Will be called when a ping response is received
        /// </summary>
        public EventHandler<PingCompletedEventArgs> OnResult { get; set; }

        public PingScan()
        {
            _pingOptions = new PingOptions(128, true);
        }

        /// <summary>
        ///     An Int32 value that specifies the maximum number of concurrent active scans
        /// </summary>
        public int MaxConcurrentScans { get; set; } = 50;

        /// <summary>
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.
        /// </summary>
        public TimeSpan TimeOut { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        ///     Indicates wether the ping operation has been completed
        /// </summary>
        public bool PingCompleted { get; private set; }

        /// <summary>
        ///     Enqueues x ip addresses for Scanning
        /// </summary>
        /// <param name="addresses"></param>
        public void Enqueue(IEnumerable<IPAddress> addresses)
        {
            var pingLimiter = _runningPingScanners;
            var initPing = _runningPingScanners;

            var cEnum = addresses.GetEnumerator();
            var lastEnumeration = false;

            while (pingLimiter <= MaxConcurrentScans && (lastEnumeration = cEnum.MoveNext()))
                if (cEnum.Current != null)
                {
                    pingLimiter++;

                    var pingSender = new Ping();
                    pingSender.PingCompleted += PingCompletedCallback;
                    pingSender.SendAsync(cEnum.Current, (int)TimeOut.TotalMilliseconds, new byte[32].Propagate((byte)'#'), _pingOptions);
                }

            Interlocked.Add(ref _runningPingScanners, pingLimiter - initPing);
            if (!lastEnumeration) return;
            lock (_lockObject)
            {
                _ipQueue.Enqueue(cEnum);
            }
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (!e.Reply.Address.Equals(default(IPAddress))) //Happens some times that the ip is 0 // && (e.Reply.Status == IPStatus.Success)
                OnResult?.Invoke(this, e);

            var nextScanAddress = GetNextAddress();

            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (nextScanAddress != default(IPAddress))
            {
                ((Ping)sender).SendAsync(nextScanAddress, (int)TimeOut.TotalMilliseconds, new byte[32].Propagate((byte)'#'), _pingOptions);
            }
            else
            {
                lock (_lockObject)
                {
                    _runningPingScanners--;

                    PingCompleted = _runningPingScanners >= 0 && _ipQueue.Count == 0;

                    if (PingCompleted)
                        OnPingFinished?.Invoke(this, new EventArgs());
                }
            }
        }

        private IPAddress GetNextAddress()
        {
            lock (_lockObject)
            {
                while (true)
                {
                    if (_currentEnumerator != null && _currentEnumerator.MoveNext())
                    {
                        return _currentEnumerator.Current;
                    }
                    else
                    {
                        if (_ipQueue.TryDequeue(out _currentEnumerator)) continue;

                        return default;
                    }
                }
            }
        }
    }
}