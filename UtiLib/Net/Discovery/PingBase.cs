using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Exceptions;

namespace UtiLib.Net.Discovery
{
    public abstract class PingBase : IDisposable
    {
        protected ConcurrentQueue<IEnumerable<IPAddress>> AddressCollectionQueue;
        protected Queue<IPAddress> AddressQueue;

        protected bool Running = false;
        private bool _measureTime;

        public bool MeasureTime
        {
            get => _measureTime;
            set
            {
                if (Running)
                    throw new InProgressException($"The property {nameof(MeasureTime)} cannot be changed as the ping is in progress");

                _measureTime = value;
            }
        }

        /// <summary>
        ///     Will be called when a ping response is received
        /// </summary>
        public EventHandler<PingCompletedEventArgs> OnResult { get; set; }

        protected PingBase()
        {
            AddressQueue = new Queue<IPAddress>();
            AddressCollectionQueue = new ConcurrentQueue<IEnumerable<IPAddress>>();
        }

        public virtual void Enqueue(IPAddress address)
        {
            if (Running)
                throw new InProgressException("Ping is in progress");

            AddressQueue.Enqueue(address);
        }

        public virtual void Enqueue(IEnumerable<IPAddress> addresses)
        {
            if (Running)
                throw new InProgressException("Ping is in progress");

            AddressCollectionQueue.Enqueue(addresses);
        }

        /// <summary>
        ///     Starts the ping process.
        ///     Enqueueing new ip addresses for ping is disabled during scantime
        /// </summary>
        public virtual void Start()
        {
            AddressCollectionQueue.Enqueue(AddressQueue);
            AddressQueue = null;
            Running = true;
        }

        protected bool _disposed;

        public virtual void Dispose()
        {
            _disposed = true;
        }
    }
}