using System;
using System.IO;

namespace UtiLib.Logging.LogProvider
{
    /// <summary>
    /// Writes logs to a basestream
    /// TODO: Async stream logger
    /// </summary>
    public abstract class StreamLogBase : LogBase, IDisposable
    {
        protected Stream LogStream;
        protected StreamWriter LogStreamWriter;

        protected bool _initialized;
        protected readonly object StreamLockObject = new object();

        internal StreamLogBase()
        {
        }

        protected StreamLogBase(Stream baseStream)
        {
            UpdateBaseStream(baseStream);
        }

        public void UpdateBaseStream(Stream baseStream)
        {
            lock (StreamLockObject)
            {
                LogStreamWriter = new StreamWriter(LogStream = baseStream, Settings.DefaultEncoding);
                _initialized = true;
            }
        }

        public void Dispose()
        {
            LogStream?.Dispose();
            LogStreamWriter?.Dispose();
        }
    }
}