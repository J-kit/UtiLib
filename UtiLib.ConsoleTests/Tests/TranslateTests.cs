using System.Diagnostics;
using System.Threading.Tasks;
using UtiLib.IO.Translation;

namespace UtiLib.ConsoleTests.Tests
{
    internal class TranslateTests
    {
        public static async Task Translate()
        {
            var source = "Hello World!";
            var translated = await Translator.TranslateAsync(source, TranslationContext.Create(Language.Automatic, Language.German));
            Logger.Log($"'{source}' translates to '{translated.Value}' in german");
        }
    }
}