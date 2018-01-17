﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.LogProvider
{
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    public class ConsoleLog : LogBase
    {
        private static readonly object Writelock = new object();

        private static readonly Dictionary<LogSeverity, ConsoleColor> ColorDict = new Dictionary<LogSeverity, ConsoleColor>
        {
            [LogSeverity.None] = ConsoleColor.White,
            [LogSeverity.Information] = ConsoleColor.Yellow,
            [LogSeverity.Warning] = ConsoleColor.Blue,
            [LogSeverity.Error] = ConsoleColor.Red,
            [LogSeverity.VerboseError] = ConsoleColor.Red,
            [LogSeverity.ErrorBreak] = ConsoleColor.Red,
        };

        public ConsoleLog() : base()
        {
        }

        public ConsoleLog(ILogFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public int GetWindowWith()
        {
            return Console.WindowWidth;
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public override void Log(string logText, LogSeverity severity)
        {
            if (!ColorDict.TryGetValue(severity, out var cc, ConsoleColor.White))
                cc = ConsoleColor.White;

            logText = FormatProvider.Format(logText);

            WriteColorful(logText, cc);

            if (Settings.AllowLogBreak && severity == LogSeverity.ErrorBreak && Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static void WriteColorful(string text, ConsoleColor color)
        {
            lock (Writelock)
            {
                if (Console.CursorLeft != 0)
                {
                    Console.WriteLine();
                }

                var bakColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = bakColor;
            }
        }
    }
}