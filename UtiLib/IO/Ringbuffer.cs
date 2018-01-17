using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtiLib.IO
{
    public interface IRingbufferProvider
    {
        int PoolBufferSize { get; }

        BufferObject Create();

        void ReIntegrate(BufferObject obj);

        int Ticks { get; }
    }

    public class RingBuffer : IRingbufferProvider
    {
        private readonly Queue<BufferObject> _ringBuffer = new Queue<BufferObject>();

        private Timer _bufferThresholdTimer;

        private int _ticks;

        public int PoolBufferSize => 2 * 1024;

        public int Ticks
        {
            get => _ticks;
            private set => _ticks = value;
        }

        public static int BufferThreshold = 100;
        public static double BufferThresholdPercentage = 1.5;

        public RingBuffer(int initialBuffers)
        {
            _bufferThresholdTimer = new Timer(BufferThresholdWatcher, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            for (var i = 0; i < initialBuffers; i++)
            {
                new BufferObject(this).ReIntegrate();
            }
        }

        private void BufferThresholdWatcher(object state)
        {
            var cTickCount = Interlocked.Increment(ref _ticks);
            var minTickCount = cTickCount - 5;

            var coldBuffers = _ringBuffer.Count(m => m.LastUseTime < minTickCount);
            if (coldBuffers > BufferThreshold * BufferThresholdPercentage)
            {
                var overflow = coldBuffers - BufferThreshold;
                for (int i = 0; i < overflow; i++)
                {
                    if (_ringBuffer.TryDequeue(out var result))
                    {
                        result.DoDispose = true;
                        result.Value = null;
                    }
                }
                GC.Collect();
                //Log.Write($"Disposed {overflow} instances of RingBuffer (Current: {_ringBuffer.Count})");
            }
        }

        public BufferObject Create()
        {
            if (!_ringBuffer.TryDequeue(out var result))
            {
                result = new BufferObject(this);
            }
            result.LastUseTime = Ticks;
            result.IsIntegrated = false;
            return result;
        }

        public void ReIntegrate(BufferObject obj)
        {
            obj.Reset(true);
            _ringBuffer.Enqueue(obj);
        }
    }

    public class ConcurrentRingBuffer : IRingbufferProvider
    {
        private readonly ConcurrentQueue<BufferObject> _ringBuffer = new ConcurrentQueue<BufferObject>();

        private Timer _bufferThresholdTimer;

        private int _ticks;

        public int PoolBufferSize => 2 * 1024;

        public int Ticks
        {
            get => _ticks;
            private set => _ticks = value;
        }

        public static int BufferThreshold = 100;
        public static double BufferThresholdPercentage = 1.5;

        public ConcurrentRingBuffer(int initialBuffers)
        {
            _bufferThresholdTimer = new Timer(BufferThresholdWatcher, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            for (var i = 0; i < initialBuffers; i++)
            {
                new BufferObject(this).ReIntegrate();
            }
        }

        private void BufferThresholdWatcher(object state)
        {
            var cTickCount = Interlocked.Increment(ref _ticks);
            var minTickCount = cTickCount - 5;

            var coldBuffers = _ringBuffer.Count(m => m.LastUseTime < minTickCount);
            if (coldBuffers > BufferThreshold * BufferThresholdPercentage)
            {
                var overflow = coldBuffers - BufferThreshold;
                for (int i = 0; i < overflow; i++)
                {
                    if (_ringBuffer.TryDequeue(out var result))
                    {
                        result.DoDispose = true;
                        result.Value = null;
                    }
                }
                GC.Collect();
                //Log.Write($"Disposed {overflow} instances of ConcurrentRingbuffer (Current: {_ringBuffer.Count})");
            }
        }

        public BufferObject Create()
        {
            if (!_ringBuffer.TryDequeue(out var result))
            {
                result = new BufferObject(this);
            }
            result.LastUseTime = Ticks;
            result.IsIntegrated = false;
            return result;
        }

        public void ReIntegrate(BufferObject obj)
        {
            obj.Reset(true);
            _ringBuffer.Enqueue(obj);
        }
    }

    public class BufferObject : IDisposable
    {
        private static int _entityCounter = 0;

        public int LastUseTime = 0;

        public int Length => High - Low;
        public int Capacity => Value.Length;
        public int FreeBytes => Value.Length - High;
        public bool IsFull => High == Value.Length;

        public readonly int EntityId = ++_entityCounter;
        public bool DoDispose = false;
        public bool IsIntegrated = false;

        public byte[] Value;
        public int High = 0;
        public int Low = 0;

        private readonly IRingbufferProvider _parent;

        internal BufferObject(IRingbufferProvider parent)
        {
            _parent = parent;
            Value = new byte[_parent.PoolBufferSize];
        }

        private BufferObject(byte[] input)
        {
            Value = input ?? new byte[_parent.PoolBufferSize];
        }

        /// <summary>
        /// Writes bytes to an array and returns the amount of written bytes
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="sourceOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Write(byte[] sourceBuffer, int sourceOffset, int count = -1)
        {
            if (count == -1)
            {
                count = sourceBuffer.Length;
            }
            var lHigh = High;
            var z1 = sourceBuffer.Length - sourceOffset;
            var z2 = Value.Length - lHigh;

            if (z1 < count)
            {
                count = z1;
            }

            if (z2 < count)
            {
                count = z2;
            }

            if (count > 0)
                Array.Copy(sourceBuffer, sourceOffset, Value, lHigh, count);

            High += count;
            return count;
        }

        public int ReadByte()
        {
            if (Length < 0)
            {
                return Value[Low++];
            }
            return -1;
        }

        public int Read(byte[] destBuffer, int destOffset, int count = -1)
        {
            if (count == -1)
                count = destBuffer.Length - destOffset;

            var cLen = Length;
            if (cLen < count)
            {
                count = cLen;
            }

            if (count > 0)
            {
                Array.Copy(Value, Low, destBuffer, destOffset, count);
                Low += count;
            }

            return count;
        }

        /// <summary>
        /// Re-Enables R/W from the beginning
        /// </summary>
        /// <param name="isIntegration"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(bool isIntegration = false)
        {
            High = 0;
            Low = 0;

            if (isIntegration)
            {
                LastUseTime = _parent.Ticks;
                IsIntegrated = true;
            }
        }

        public BufferObject Clone(bool optimize = false)
        {
            var res = _parent.Create();
            var len = Length;
            if (optimize)
            {
                res.Low = 0;
                res.High = len;
            }
            else
            {
                res.Low = Low;
                res.High = High;
            }
            Array.Copy(this.Value, this.Low, res.Value, res.Low, len);
            return res;
        }

        public void Dispose()
        {
            Reset();
        }

        public void ReIntegrate()
        {
            _parent.ReIntegrate(this);
        }
    }
}