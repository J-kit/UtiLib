using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace UtiLib.Net.Discovery
{
    public class ApiPingNew : PingBase
    {
        private readonly object _lockObject = new object();

        private readonly PingOptions _pingOptions;
        private IEnumerator<IPAddress> _currentEnumerator;
        private int _runningPingScanners;

        /// <summary>
        ///     Will be called when each ping has been completed
        /// </summary>
        public EventHandler OnPingFinished { get; set; }

        public ApiPingNew()
        {
            _pingOptions = new PingOptions(128, true);
        }

        public TimeSpan TimeOut { get; set; } = TimeSpan.MaxValue;
        public int MaxConcurrentScans { get; set; } = 50;

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
            if (!e.Reply.Address.Equals(default(IPAddress))) //Happens some times that the ip is 0 // && (e.Reply.Status == IPStatus.Success)
            {
                OnResult?.Invoke(this, e);
            }
            else
            {
                Logger.Log($"Invalid pingreply from pingObject");
            }

            var nextScanAddress = GetNext();
            if (nextScanAddress != null)
            {
                ((Ping)sender).SendAsync(nextScanAddress, (int)TimeOut.TotalMilliseconds, new byte[32].Propagate((byte)'#'), _pingOptions);
            }
            else
            {
                //TODO
                lock (_lockObject)
                {
                    _runningPingScanners--;

                    var pingCompleted = _runningPingScanners >= 0 && AddressCollectionQueue.IsEmpty;

                    if (pingCompleted)
                        OnPingFinished?.Invoke(this, new EventArgs());
                }
            }
        }

        private IPAddress GetNext()
        {
            lock (_lockObject)
            {
                if (_currentEnumerator.MoveNext()) return _currentEnumerator.Current;

                _currentEnumerator.Dispose();
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