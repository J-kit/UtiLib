using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtiLib.Logging.LogProvider;

namespace UtiLib.Environment
{
    public enum EnvironmentDefinition
    {
        Default,
        Console,
        Gui,
        Service,
    }

    public class Dedection
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private static EnvironmentDefinition Definition = EnvironmentDefinition.Default;

        public static EnvironmentDefinition CurrentEnvironment =>
            (Definition != EnvironmentDefinition.Default)
            ? Definition
            : (Definition = GetCurrentEnvironment());

        public static EnvironmentDefinition GetCurrentEnvironment()
        {
            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return HandleWindows();

                case PlatformID.Unix:
                    return HandleUnix();

                case PlatformID.MacOSX:
                case PlatformID.Xbox:
                default:
                    return EnvironmentDefinition.Default;
            }
        }

        /// <summary>
        /// Determines the current machine's state
        /// Warning: If executed on windows it will return an invalid result!
        /// </summary>
        /// <returns></returns>
        private static EnvironmentDefinition HandleUnix()
        {
            if (Console.In is StreamReader) //System.Environment.UserInteractive doesn't do its job
            {
                return EnvironmentDefinition.Console;
            }
            else
            {
                return EnvironmentDefinition.Service;
            }
        }

        private static EnvironmentDefinition HandleWindows()
        {
            if (!System.Environment.UserInteractive)
            {
                return EnvironmentDefinition.Service;
            }

            if (GetConsoleWindow() != IntPtr.Zero)
            {
                return EnvironmentDefinition.Console;
            }
            else
            {
                return EnvironmentDefinition.Gui;
            }
        }

        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}