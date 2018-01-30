using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UtiLib.Exceptions;

namespace UtiLib.Net.Discovery
{
    public abstract class PingBase : IDisposable
    {
        protected readonly object LockObject = new object();

        protected bool _disposed;
        private bool _measureTime;

        protected ConcurrentQueue<IEnumerable<IPAddress>> AddressCollectionQueue;
        protected Queue<IPAddress> AddressQueue;

        protected bool Running;

        protected PingBase()
        {
            AddressQueue = new Queue<IPAddress>();
            AddressCollectionQueue = new ConcurrentQueue<IEnumerable<IPAddress>>();
        }

        public bool MeasureTime
        {
            get => _measureTime;
            set
            {
                if (Running)
                    throw new InProgressException(
                        $"The property {nameof(MeasureTime)} cannot be changed as the ping is in progress");

                _measureTime = value;
            }
        }

        /// <summary>
        ///     Will be called when each ping has been completed
        /// </summary>
        public EventHandler OnPingFinished { get; set; }

        /// <summary>
        ///     Will be called when a ping response is received
        /// </summary>
        public EventHandler<PingCompletedEventArgs> OnResult { get; set; }

        /// <summary>
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the
        ///     ICMP echo reply message.
        /// </summary>
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public virtual void Dispose()
        {
            _disposed = true;
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

        /// <summary>
        /// Starts and waits for the last ping response/timeout
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IReadOnlyCollection<IPAddress>> StartAsync()
        {
            var lockObj = new object();
            var ipHs = new HashSet<IPAddress>();

            var tcs = new TaskCompletionSource<IReadOnlyCollection<IPAddress>>();
            OnPingFinished += (_, __) => tcs.SetResult(ipHs);
            OnResult += (_, args) =>
            {
                lock (lockObj)
                {
                    ipHs.Add(args.Reply.Address);
                }
            };
            Start();

            return await tcs.Task;
        }

        public void Prepare(PingEngineCreationFlags flags = PingEngineCreationFlags.Default)
        {
            MeasureTime = flags.HasFlag(PingEngineCreationFlags.MeasureTime);

            if (flags.HasFlag(PingEngineCreationFlags.Subnet))
            {
                Enqueue(NetMaskHelper.RetrieveSubnetAddresses());
            }
        }
    }
}