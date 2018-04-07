using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UtiLib.IO
{
    public struct MultiStreamProcessChangedEventArgs
    {
        public long CurrentPosition { get; set; }
        public long Length { get; set; }

        public MultiStreamProcessChangedEventArgs(long length, long currentPosition)
        {
            CurrentPosition = currentPosition;
            Length = length;
        }

        public void Deconstruct(out long length, out long currentPosition)
        {
            currentPosition = CurrentPosition;
            length = Length;
        }
    }

    /// <summary>
    /// http://www.c-sharpcorner.com/article/combine-multiple-streams-in-a-single-net-framework-stream-o/
    /// </summary>
    public class MultiStream : Stream
    {
        private readonly List<Stream> _streamList = new List<Stream>();
        private long _position = 0;

        public EventHandler<MultiStreamProcessChangedEventArgs> OnPositionChanged;

        public MultiStream(params Stream[] streams)
        {
            foreach (var stream in streams)
            {
                AddStream(stream);
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _length;
        private long _length;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long len = Length;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;

                case SeekOrigin.Current:
                    _position += offset;
                    break;

                case SeekOrigin.End:
                    _position = len - offset;
                    break;
            }
            if (_position > len)
            {
                _position = len;
            }
            else if (_position < 0)
            {
                _position = 0;
            }
            return _position;
        }

        public override void SetLength(long value)
        {
        }

        public void AddStream(Stream stream)
        {
            _streamList.Add(stream);
            _length += stream.Length; // _streamList.Sum(x => x.Length);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long len = 0;
            int result = 0;
            int bufPos = offset;
            int bytesRead;
            foreach (var stream in _streamList)
            {
                if (_position < (len + stream.Length))
                {
                    stream.Position = _position - len;
                    bytesRead = stream.Read(buffer, bufPos, count);
                    result += bytesRead;
                    bufPos += bytesRead;
                    _position += bytesRead;
                    if (bytesRead < count)
                    {
                        count -= bytesRead;
                    }
                    else
                    {
                        break;
                    }
                }
                len += stream.Length;
            }
            OnPositionChanged?.Invoke(this, new MultiStreamProcessChangedEventArgs(Length, _position));
            return result;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }
    }
}