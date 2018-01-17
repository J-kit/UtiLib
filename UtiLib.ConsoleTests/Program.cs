using System;
using UtiLib.Environment;
using UtiLib.Logging;
using UtiLib.Logging.LogProvider;
using UtiLib.Shared.Enums;

namespace UtiLib.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Current Environment: {Dedection.CurrentEnvironment}/{System.Environment.OSVersion}");
            LogExample();
            MulticastLogExample();
            Console.ReadLine();
        }

        /// <summary>
        /// Letting the Library chooce which Logger to choose (Console, Debug (or File?))
        /// </summary>
        private static void LogExample()
        {
            var cLogger = LogFactory.DefaultLogger;
            cLogger.Log("A really serious error!", LogSeverity.Error);
        }

        /// <summary>
        /// Example how to multicast logs to several destinations (File, Console, Debug log,...)
        /// </summary>
        private static void MulticastLogExample()
        {
            var cLogger = new MulticastLogger(new ConsoleLogger(), new DebugLogger());
            cLogger.Log("Something went wrong!", LogSeverity.Warning);
        }
    }
}// new ConsoleLogger(), new DebugLogger() 