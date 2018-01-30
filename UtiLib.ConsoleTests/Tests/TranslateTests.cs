using System.Diagnostics;
using UtiLib.IO.Translate;

namespace UtiLib.ConsoleTests.Tests
{
    internal class TranslateTests
    {
        public static void Translate()
        {
            //  var trans = new Translator
            var res = Translator.Translate("Hello World!", "auto", "de");
            Debugger.Break();
        }
    }
}