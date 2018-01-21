using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace UtiLib.Net.Discovery
{
    public class PingScan : IPingScaner
    {
        private readonly Queue<IEnumerator<IPAddress>> _ipQueue = new Queue<IEnumerator<IPAddress>>();

        private readonly object _lockObject = new object();
        private readonly PingOptions _pingOptions;
        private IEnumerator<IPAddress> _currentEnumerator;
        private int _runningPingScanners;

        public EventHandler OnPingFinished { get; set; }
        public EventHandler<PingCompletedEventArgs> OnResult { get; set; }

        public PingScan()
        {
            _pingOptions = new PingOptions(128, true);
            //  _scanSuccessCallback = scanSuccessCallback;
        }

        public int MaxConcurrentScans { get; set; } = 50;
        public TimeSpan TimeOut { get; set; }

        public bool PingCompleted { get; private set; }

        /// <summary>
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
            if (!e.Reply.Address.Equals(default(IPAddress))) //Happens some times that the ip is 0
                if (e.Reply.Status == IPStatus.Success)
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