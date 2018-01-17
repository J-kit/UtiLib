using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtiLib.Logging.LogProvider;
using UtiLib.Shared.Enums;

namespace UtilLibTest
{
    [TestClass]
    public class LogTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var cLogger = new ConsoleLogger();
            cLogger.Log("Yu", LogSeverity.Warning);
            Debugger.Break();
        }
    }
}