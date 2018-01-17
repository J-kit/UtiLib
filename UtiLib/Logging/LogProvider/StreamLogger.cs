using System;
using UtiLib.Shared.Enums;

namespace UtiLib.Logging.LogProvider
{
    /// <inheritdoc />
    /// <summary>
    /// Writes logs to a basestream
    /// </summary>
    public class StreamLogger : StreamLogBase
    {
        public override void Log(string logText, LogSeverity severity)
        {
            if (!_initialized)
                throw new TypeInitializationException("Log provider not initialized", new Exception("UpdateBaseStream wasn't called"));

            lock (StreamLockObject)
            {
                logText = FormatProvider.Format($"[{severity}] {logText}");
                LogStreamWriter.WriteLine(logText);
            }
        }
    }
}