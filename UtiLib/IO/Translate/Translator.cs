// Copyright (c) 2015 Ravi Bhavnani
// License: Code Project Open License
// http://www.codeproject.com/info/cpol10.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UtiLib.IO.Translate
{
    /// <summary>
    ///     Translates text using Google's online language tools.
    /// </summary>
    public class Translator
    {
        #region Fields

        /// <summary>
        ///     The language to translation mode map.
        /// </summary>
        private static readonly Dictionary<Languages, string> LanguageModeMap;

        #endregion Fields

        static Translator()
        {
            LanguageModeMap = new Dictionary<Languages, string>
            {
                { Languages.Automatic, "auto" },
                { Languages.Afrikaans, "af" },
                { Languages.Albanian, "sq" },
                { Languages.Arabic, "ar" },
                { Languages.Armenian, "hy" },
                { Languages.Azerbaijani, "az" },
                { Languages.Basque, "eu" },
                { Languages.Belarusian, "be" },
                { Languages.Bengali, "bn" },
                { Languages.Bulgarian, "bg" },
                { Languages.Catalan, "ca" },
                { Languages.Chinese, "zh-CN" },
                { Languages.Croatian, "hr" },
                { Languages.Czech, "cs" },
                { Languages.Danish, "da" },
                { Languages.Dutch, "nl" },
                { Languages.English, "en" },
                { Languages.Esperanto, "eo" },
                { Languages.Estonian, "et" },
                { Languages.Filipino, "tl" },
                { Languages.Finnish, "fi" },
                { Languages.French, "fr" },
                { Languages.Galician, "gl" },
                { Languages.German, "de" },
                { Languages.Georgian, "ka" },
                { Languages.Greek, "el" },
                { Languages.Haitian_Creole, "ht" },
                { Languages.Hebrew, "iw" },
                { Languages.Hindi, "hi" },
                { Languages.Hungarian, "hu" },
                { Languages.Icelandic, "is" },
                { Languages.Indonesian, "id" },
                { Languages.Irish, "ga" },
                { Languages.Italian, "it" },
                { Languages.Japanese, "ja" },
                { Languages.Korean, "ko" },
                { Languages.Lao, "lo" },
                { Languages.Latin, "la" },
                { Languages.Latvian, "lv" },
                { Languages.Lithuanian, "lt" },
                { Languages.Macedonian, "mk" },
                { Languages.Malay, "ms" },
                { Languages.Maltese, "mt" },
                { Languages.Norwegian, "no" },
                { Languages.Persian, "fa" },
                { Languages.Polish, "pl" },
                { Languages.Portuguese, "pt" },
                { Languages.Romanian, "ro" },
                { Languages.Russian, "ru" },
                { Languages.Serbian, "sr" },
                { Languages.Slovak, "sk" },
                { Languages.Slovenian, "sl" },
                { Languages.Spanish, "es" },
                { Languages.Swahili, "sw" },
                { Languages.Swedish, "sv" },
                { Languages.Tamil, "ta" },
                { Languages.Telugu, "te" },
                { Languages.Thai, "th" },
                { Languages.Turkish, "tr" },
                { Languages.Ukrainian, "uk" },
                { Languages.Urdu, "ur" },
                { Languages.Vietnamese, "vi" },
                { Languages.Welsh, "cy" },
                { Languages.Yiddish, "yi" },
            };
        }

        #region Private methods

        /// <summary>
        ///     Converts a language to its identifier.
        /// </summary>
        /// <param name="language">The language."</param>
        /// <returns>The identifier or <see cref="string.Empty" /> if none.</returns>
        private static string ResolveLanguage(string language)
        {
            var dLang = language.ToLower();
            if (Enum.TryParse<Languages>(dLang, true, out var enumValue) && LanguageModeMap.TryGetValue(enumValue, out var mode))
            {
                return mode;
            }

            return LanguageModeMap.Values.Contains(dLang) ? dLang : string.Empty;
        }

        private static string TranslateWords(string inputText)
        {
            var startQuote = inputText.IndexOf('\"');
            if (startQuote != -1)
            {
                var endQuote = inputText.IndexOf('\"', startQuote + 1);
                if (endQuote != -1) return inputText.Substring(startQuote + 1, endQuote - startQuote - 1);
            }

            return string.Empty;
        }

        private static string TranslatePhrases(int index, string text)
        {
            var sb = new StringBuilder();

            // Translation of phrase
            text = text.Substring(0, index)
                .Replace("],[", ",")
                .Replace("]", string.Empty)
                .Replace("[", string.Empty)
                .Replace("\",\"", "\"");

            // Get translated phrases
            var phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < phrases.Length; i += 2)
            {
                var translatedPhrase = phrases[i];
                if (translatedPhrase.StartsWith(",,"))
                {
                    i--;
                    continue;
                }

                sb.Append(translatedPhrase);
                sb.Append("  ");
            }

            return sb.ToString();
        }

        private static string DownloadString(string url)
        {
            using (var wc = new WebClient().PrepareClient())
            {
                return wc.DownloadString(url);
            }
        }

        private async Task<string> DownloadStringTaskAsync(string url)
        {
            using (var wc = new WebClient().PrepareClient())
            {
                return await wc.DownloadStringTaskAsync(url);
            }
        }

        #endregion Private methods

        #region Public methods

        /// <summary>
        ///     Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>The translation.</returns>
        public static string Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            // Initialize
            var realSourceLang = ResolveLanguage(sourceLanguage);
            var realTargetLang = ResolveLanguage(targetLanguage);

            // Download translation
            var text = DownloadString($"https://translate.googleapis.com/translate_a/single?client=gtx&sl={realSourceLang}&tl={realTargetLang}&dt=t&q={HttpUtility.UrlEncode(sourceText)}");

            var index = text.IndexOf($",,\"{realTargetLang}\"", StringComparison.Ordinal);
            var translationResult = index == -1 ? TranslateWords(text) : TranslatePhrases(index, text);

            // Fix up translation
            translationResult = translationResult.Trim()
                .Replace(" .", ".").Replace(" ?", "?").Replace(" !", "!")
                .Replace(" ,", ",").Replace(" ;", ";");

            // And translation speech URL

            return translationResult;
        }

        public static string GetTTs(string inputText, string targetLanguage)
        {
            var realTargetLang = ResolveLanguage(targetLanguage);

            return
                $"https://translate.googleapis.com/translate_tts?ie=UTF-8&q={HttpUtility.UrlEncode(inputText)}&tl={realTargetLang}&total=1&idx=0&textlen={inputText.Length}&client=gtx";
        }

        #endregion Public methods

        #region Properties

        ///// <summary>
        /////     Gets the supported languages.
        ///// </summary>
        //public static IEnumerable<string> Languages
        //{
        //    get { return LanguageModeMap.Keys.OrderBy(p => p); }
        //}

        /// <summary>
        ///     Gets the url used to speak the translation.
        /// </summary>
        /// <value>The url used to speak the translation.</value>
        public string TranslationSpeechUrl { get; private set; }

        #endregion Properties
    }
}