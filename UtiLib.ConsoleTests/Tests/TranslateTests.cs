using System.Diagnostics;
using UtiLib.IO.Translation;

namespace UtiLib.ConsoleTests.Tests
{
    internal class TranslateTests
    {
        public static void Translate()
        {
            //  var trans = new Translator
            var res = Translator.Translate("Hello World!", TranslationContext.Create(Language.Automatic, Language.German));
            Debugger.Break();
        }
    }
}