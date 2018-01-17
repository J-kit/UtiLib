using System.Diagnostics;
using UtiLib.Shared.Enums;

namespace UtiLib.Logging.LogProvider
{
    public class DebugLogger : LogBase
    {
        public override void Log(string logText, LogSeverity severity)
        {
            Debug.WriteLine(FormatProvider.Format($"[{severity}] {logText}"));
        }
    }
}