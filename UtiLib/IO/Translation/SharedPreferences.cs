using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UtiLib.IO.Translation
{
    internal class SharedPreferences
    {
        static SharedPreferences()
        {
            var llmap = new Dictionary<Language, string>
            {
                { Language.Automatic, "auto" },
                { Language.Afrikaans, "af" },
                { Language.Albanian, "sq" },
                { Language.Arabic, "ar" },
                { Language.Armenian, "hy" },
                { Language.Azerbaijani, "az" },
                { Language.Basque, "eu" },
                { Language.Belarusian, "be" },
                { Language.Bengali, "bn" },
                { Language.Bulgarian, "bg" },
                { Language.Catalan, "ca" },
                { Language.Chinese, "zh-CN" },
                { Language.Croatian, "hr" },
                { Language.Czech, "cs" },
                { Language.Danish, "da" },
                { Language.Dutch, "nl" },
                { Language.English, "en" },
                { Language.Esperanto, "eo" },
                { Language.Estonian, "et" },
                { Language.Filipino, "tl" },
                { Language.Finnish, "fi" },
                { Language.French, "fr" },
                { Language.Galician, "gl" },
                { Language.German, "de" },
                { Language.Georgian, "ka" },
                { Language.Greek, "el" },
                { Language.Haitian_Creole, "ht" },
                { Language.Hebrew, "iw" },
                { Language.Hindi, "hi" },
                { Language.Hungarian, "hu" },
                { Language.Icelandic, "is" },
                { Language.Indonesian, "id" },
                { Language.Irish, "ga" },
                { Language.Italian, "it" },
                { Language.Japanese, "ja" },
                { Language.Korean, "ko" },
                { Language.Lao, "lo" },
                { Language.Latin, "la" },
                { Language.Latvian, "lv" },
                { Language.Lithuanian, "lt" },
                { Language.Macedonian, "mk" },
                { Language.Malay, "ms" },
                { Language.Maltese, "mt" },
                { Language.Norwegian, "no" },
                { Language.Persian, "fa" },
                { Language.Polish, "pl" },
                { Language.Portuguese, "pt" },
                { Language.Romanian, "ro" },
                { Language.Russian, "ru" },
                { Language.Serbian, "sr" },
                { Language.Slovak, "sk" },
                { Language.Slovenian, "sl" },
                { Language.Spanish, "es" },
                { Language.Swahili, "sw" },
                { Language.Swedish, "sv" },
                { Language.Tamil, "ta" },
                { Language.Telugu, "te" },
                { Language.Thai, "th" },
                { Language.Turkish, "tr" },
                { Language.Ukrainian, "uk" },
                { Language.Urdu, "ur" },
                { Language.Vietnamese, "vi" },
                { Language.Welsh, "cy" },
                { Language.Yiddish, "yi" },
            };
            LanguageMap = new ConcurrentDictionary<Language, string>(llmap);
        }

        /// <summary>
        ///     The language to translation mode map.
        /// </summary>
        internal static readonly ConcurrentDictionary<Language, string> LanguageMap;

        public const string BaseUrl = "https://translate.googleapis.com/";
    }
}