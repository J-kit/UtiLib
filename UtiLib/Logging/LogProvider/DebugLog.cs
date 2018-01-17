using System.Diagnostics;

namespace UtiLib.Logging.LogProvider
{
    public class DebugLog : LogBase
    {
        public override void Log(string logText, LogSeverity severity)
        {
            Debug.WriteLine(FormatProvider.Format($"[{severity}] {logText}"));
        }
    }
}