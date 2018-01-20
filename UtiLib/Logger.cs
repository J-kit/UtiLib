using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Shared.Enums;

namespace UtiLib
{
    public class Logger
    {
        public static void Log(string logText, LogSeverity severity) => Settings.Logger.Log(logText, severity);

        public static void Log(object logText, LogSeverity severity = LogSeverity.Information) => Log(logText.ToString(), severity);

        public static void Log(Exception ex)
        {
            var sb = new StringBuilder();
            var with = Settings.Logger.WindowWith;
            for (var i = 0; i < with; i++)
            {
                sb.Append('=');
            }

            Log($"\n{sb}\nAn Exception occured! Details: \n {ex}\n{sb}", LogSeverity.Error);
        }
    }
}