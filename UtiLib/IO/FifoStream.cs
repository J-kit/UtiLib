using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UtiLib.IO
{
    [DebuggerDisplay("Length = {Length}")]
    public class FifoStream : Stream, IDisposable
    {
        private readonly IRingbufferProvider _ringBufferProvider;
        private readonly Queue<BufferObject> _localBuffer = new Queue<BufferObject>();

        private long _length;
        private BufferObject _lastBuffer;

        public FifoStream(IRingbufferProvider provider)
        {
            _ringBufferProvider = provider;
        }

        // public static int BufferSize => ConcurrentRingbuffer1.DefaultBufferSize;

        public int WastedBytes => _localBuffer.Sum(m => m.Value.Length - m.High);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { get; set; }

        public bool IsEmpty => _localBuffer.Count == 0;

        public new void Dispose()
        {
            _lastBuffer = null;
            while (_localBuffer?.Count != 0)
                _localBuffer?.Dequeue().ReIntegrate();

            base.Dispose();
        }

        /// <summary>
        ///     Moves the required amount of bytes onto a new fifostream
        ///     Warning: Unused buffer segments can appear
        /// </summary>
        /// <param name="requiredBytes"></param>
        /// <returns></returns>
        public FifoStream FastMove(int requiredBytes)
        {
            _lastBuffer = null;

            var dStream = new FifoStream(_ringBufferProvider);

            while (requiredBytes > 0 && _length > 0 && _localBuffer.Count > 0)
            {
                var cBuf = _localBuffer.Peek();
                var cBufLen = cBuf.Length;

                if (cBufLen <= requiredBytes)
                {
                    dStream._localBuffer.Enqueue(cBuf);
                    dStream._length += cBufLen;
                    requiredBytes -= cBufLen;
                    _length -= cBufLen;
                    _localBuffer.Dequeue();
                }
                else
                {
                    var nBuf = _ringBufferProvider.Create();
                    var readBytes =
                        nBuf.Write(cBuf.Value, cBuf.Low, requiredBytes);

                    cBuf.Low += readBytes;
                    _length -= readBytes;
                    requiredBytes -= readBytes;

                    dStream._length += readBytes;
                    dStream._localBuffer.Enqueue(nBuf);

                    if (cBuf.Length == 0) _localBuffer.Dequeue().ReIntegrate();
                }
            }

            return dStream;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            BufferObject cBuf = null;

            var tCount = count;
            var cOffset = offset;
            var rBytes = 0;

            _lastBuffer = null;

            do
            {
                if (_localBuffer.Count == 0) break;

                cBuf = _localBuffer.Peek();

                var readBytes = cBuf.Read(buffer, cOffset, tCount);
                if (readBytes > 0)
                {
                    tCount -= readBytes;
                    _length -= readBytes;

                    cOffset += readBytes;
                    rBytes += readBytes;
                }
                else if (readBytes == 0)
                {
                    if (cBuf.Length == 0)
                    {
                        if (_localBuffer.Dequeue().EntityId != cBuf.EntityId)
                            Debugger
                                .Break(); // Log.Write("[FifoStream] Entity id's don't match! (Parallel usage?)", LogSeverity.ErrorBreak);

                        cBuf.ReIntegrate();
                        cBuf = null;
                        continue;
                    }

                    break;
                }
            } while (_length > 0);

            return rBytes;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return;

            count = Math.Min(buffer.Length - offset, count);

            var tCount = count;
            var tOffset = offset;
            var cWriteBuf = _lastBuffer;

            if (cWriteBuf == null)
                _localBuffer.Enqueue(cWriteBuf = _ringBufferProvider.Create());

            while (tCount > 0)
            {
                if (cWriteBuf.IsFull)
                    _localBuffer.Enqueue(cWriteBuf = _ringBufferProvider.Create());

                var cWrite = cWriteBuf.Write(buffer, tOffset, tCount);

                tCount -= cWrite;
                tOffset += cWrite;
            }

            _lastBuffer = cWriteBuf.IsFull ? null : cWriteBuf;

            _length += count;
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Coies the current stream onto the given one
        /// </summary>
        /// <param name="stream"></param>
        public void MoveTo(FifoStream stream)
        {
            stream._lastBuffer = _lastBuffer;
            _lastBuffer = null;
            var count = _localBuffer.Count;

            for (var i = 0; i < count; i++)
                stream._localBuffer.Enqueue(_localBuffer.Dequeue());

            stream._length += _length;
            _length = 0;
            _localBuffer.Clear();
        }

        ~FifoStream()
        {
            Dispose();
        }

        #region NotImplemented

        public override void Flush()
        {
            //throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        #endregion NotImplemented
    }
}