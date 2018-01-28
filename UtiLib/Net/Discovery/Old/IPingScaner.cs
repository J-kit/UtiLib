using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace UtiLib.Net.Discovery
{
    public interface IPingScaner
    {
        int MaxConcurrentScans { get; set; }
        bool PingCompleted { get; }
        TimeSpan TimeOut { get; set; }

        void Enqueue(IEnumerable<IPAddress> addresses);

        EventHandler OnPingFinished { get; set; }
        EventHandler<PingCompletedEventArgs> OnResult { get; set; }
    }
}