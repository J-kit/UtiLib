using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Exceptions;

namespace UtiLib.Net.Discovery
{
    public abstract class PingBase
    {
        protected ConcurrentQueue<IEnumerable<IPAddress>> AddressCollectionQueue;
        protected Queue<IPAddress> AddressQueue;

        protected bool Running = false;

        public bool MeasureTime { get; set; }

        protected PingBase()
        {
            AddressQueue = new Queue<IPAddress>();
            AddressCollectionQueue = new ConcurrentQueue<IEnumerable<IPAddress>>();
        }

        public virtual void Enqueue(IPAddress address)
        {
            if (Running)
                throw new InProgressException("Ping is already in progress");

            AddressQueue.Enqueue(address);
        }

        public virtual void Enqueue(IEnumerable<IPAddress> addresses)
        {
            if (Running)
                throw new InProgressException("Ping is already in progress");

            AddressCollectionQueue.Enqueue(addresses);
        }

        public virtual void Start()
        {
            AddressCollectionQueue.Enqueue(AddressQueue);
            AddressQueue = null;
            Running = true;
        }
    }
}