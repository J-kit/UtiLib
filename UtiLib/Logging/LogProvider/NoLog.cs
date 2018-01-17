using UtiLib.Shared.Enums;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.LogProvider
{
    internal class NoLog : LogBase
    {
        public NoLog() : base()
        {
        }

        public NoLog(ILogFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public override void Log(string logText, LogSeverity severity)
        {
        }
    }
}