using UtiLib.Logging;
using UtiLib.Shared.Enums;

namespace UtiLib.Shared.Interfaces
{
    public interface ILogger
    {
        ILogFormatProvider FormatProvider { get; set; }

        void Log(string logText, LogSeverity severity);
    }
}