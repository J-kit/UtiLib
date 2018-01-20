using UtiLib.Logging.FormatProvider;
using UtiLib.Shared.Enums;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging
{
    public abstract class LogBase : ILogger
    {
        public virtual ILogFormatProvider FormatProvider { get; set; }

        protected LogBase()
        {
            FormatProvider = new DefaultFormatProvider();
        }

        protected LogBase(ILogFormatProvider formatProvider)
        {
            FormatProvider = formatProvider ?? new DefaultFormatProvider();
        }

        public abstract void Log(string logText, LogSeverity severity);

        public virtual int WindowWith { get; } = 16;
    }
}