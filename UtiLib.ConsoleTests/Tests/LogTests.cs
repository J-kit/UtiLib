using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Logging;
using UtiLib.Logging.LogProvider;
using UtiLib.Shared.Enums;

namespace UtiLib.ConsoleTests.Tests
{
    public static class LogTests
    {
        public static void DoTest()
        {
            LogExample();
            MulticastLogExample();
        }

        /// <summary>
        /// Letting the Library chooce which Logger to choose (Console, Debug (or File?))
        /// </summary>
        public static void LogExample()
        {
            var cLogger = LogFactory.DefaultLogger;
            cLogger.Log("A really serious error!", LogSeverity.Error);
        }

        /// <summary>
        /// Example how to multicast logs to several destinations (File, Console, Debug log,...)
        /// </summary>
        public static void MulticastLogExample()
        {
            var cLogger = new MulticastLogger(new ConsoleLogger(), new DebugLogger());
            cLogger.Log("Something went wrong!", LogSeverity.Warning);
        }
    }
}