using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Shared.Enums;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.LogProvider
{
    /// <summary>
    /// Allows you to log do different outputs with one call
    /// Is guaranteed to be Threadsafe
    /// </summary>
    public class MulticastLogger : LogBase
    {
        private readonly List<ILogger> _loggers;
        private readonly object _lockObject = new object();

        public ILogger this[int key]
        {
            get => SafeAccess(key);
            set => SafeAccess(key, value);
        }

        private ILogger SafeAccess(int key)
        {
            lock (_lockObject)
                return _loggers[key];
        }

        private void SafeAccess(int key, ILogger logger)
        {
            lock (_lockObject)
                _loggers[key] = logger;
        }

        public MulticastLogger()
        {
            _loggers = new List<ILogger>();
        }

        public MulticastLogger(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers as List<ILogger> ?? loggers.ToList();
        }

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
                foreach (var logger in _loggers)
                {
                    logger.Log(logText, severity);
                }
            }
        }
    }
}