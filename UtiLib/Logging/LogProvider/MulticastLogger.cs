using System.Collections.Generic;
using System.Linq;
using UtiLib.Shared.Enums;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.LogProvider
{
    /// <summary>
    /// Allows you to log do different <see cref="ILogger"/> with a single call
    /// Is guaranteed to be Threadsafe
    /// </summary>
    public class MulticastLogger : LogBase
    {
        private readonly List<ILogger> _loggers;
        private readonly object _lockObject = new object();

        /// <summary>
        ///     Is not called by <see cref="MulticastLogger"/> but by its <see cref="ILogger"/>
        /// </summary>
        public override ILogFormatProvider FormatProvider
        {
            get => base.FormatProvider;
            set
            {
                lock (_lockObject)
                {
                    base.FormatProvider = value;
                    _loggers?.ForEach(x => x.FormatProvider = value);
                }
            }
        }

        /// <summary>
        /// Access to each Logger added
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILogger this[int key]
        {
            get => SafeAccess(key);
            set => SafeAccess(key, value);
        }

        private ILogger SafeAccess(int key, ILogger logger = default)
        {
            lock (_lockObject)
            {
                return logger == default(ILogger) ? _loggers[key] : (_loggers[key] = logger);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MulticastLogger"/> class that contains no subscribers
        /// </summary>
        public MulticastLogger()
        {
            _loggers = new List<ILogger>();
        }

        /// <summary>
        ///   	 Initializes a new instance of the <see cref="MulticastLogger"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="loggers">Set of loggers which logs are broadcasted to</param>
        public MulticastLogger(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers as List<ILogger> ?? loggers.ToList();
            _loggers?.ForEach(x => x.FormatProvider = FormatProvider);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MulticastLogger"/> class that contains elements copied from the specified params.
        /// </summary>
        /// <param name="loggers"></param>
        public MulticastLogger(params ILogger[] loggers)
        {
            _loggers = loggers.ToList();
            _loggers?.ForEach(x => x.FormatProvider = FormatProvider);
        }

        /// <summary>
        ///     Adds a <see cref="ILogger"/> to the broadcast subscription list
        /// </summary>
        /// <param name="logger"></param>
        public void AddLogger(ILogger logger)
        {
            lock (_lockObject)
                _loggers.Add(logger);
        }

        public void RemoveLogger(ILogger logger)
        {
            lock (_lockObject)
                _loggers.Remove(logger);
        }

        public override void Log(string logText, LogSeverity severity)
        {
            lock (_lockObject)
            {
                _loggers.ForEach(x => x.Log(logText, severity));
            }
        }
    }
}