using UtiLib.Logging;

namespace UtiLib.Shared.Interfaces
{
    public interface ILogger
    {
        ILogFormatProvider FormatProvider { get; set; }

        void Log(string logText, LogSeverity severity);
    }
}