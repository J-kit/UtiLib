using UtiLib.Environment;
using UtiLib.Logging.LogProvider;
using UtiLib.Shared.Enums;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging
{
    public class LogFactory
    {
        private static ILogger _defaultLogger;

        public static ILogger DefaultLogger => _defaultLogger ?? (_defaultLogger = InternalGetProvider());

        private static ILogger InternalGetProvider()
        {
            switch (Dedection.CurrentEnvironment)
            {
                case EnvironmentDefinition.Console:
                    return new ConsoleLogger();

                case EnvironmentDefinition.Gui:
                    return new DebugLogger();

                case EnvironmentDefinition.Default:
                case EnvironmentDefinition.Service:
                default:
                    return new NoLog();
            }
        }
    }
}