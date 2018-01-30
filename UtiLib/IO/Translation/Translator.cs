// Copyright (c) 2015 Ravi Bhavnani
// License: Code Project Open License
// http://www.codeproject.com/info/cpol10.aspx

using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UtiLib.IO.Translation
{
    /// <summary>
    ///     Translates text using Google's online language tools.
    /// </summary>
    public class Translator
    {
        #region Private methods

        ///// <summary>
        /////     Converts a language to its identifier.
        ///// </summary>
        ///// <param name="language">The language."</param>
        ///// <returns>The identifier or <see cref="string.Empty" /> if none.</returns>
        //internal static string ResolveLanguage(string language)
        //{
        //    var dLang = language.ToLower();
        //    var langMap = SharedPreferences.LanguageMap;

        //    if (Enum.TryParse<Language>(dLang, true, out var enumValue) && langMap.TryGetValue(enumValue, out var mode))
        //    {
        //        return mode;
        //    }

        //    return langMap.Values.Contains(dLang) ? dLang : string.Empty;
        //}

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
        /// <param name="context"></param>
        /// <returns>The translation.</returns>
        public static TranslatorResult Translate(string sourceText, TranslationContext context)
        {
            var realSourceLang = context.SourceLanguage.ToShortForm();
            var realTargetLang = context.DestinationLanguage.ToShortForm();

            // Download translation
            var text = DownloadString($"https://translate.googleapis.com/translate_a/single?client=gtx&sl={realSourceLang}&tl={realTargetLang}&dt=t&q={HttpUtility.UrlEncode(sourceText)}");

            var index = text.IndexOf($",,\"{realTargetLang}\"", StringComparison.Ordinal);
            var translationResult = index == -1 ? TranslateWords(text) : TranslatePhrases(index, text);

            // Fix up translation
            translationResult = translationResult.Trim()
                .Replace(" .", ".").Replace(" ?", "?").Replace(" !", "!")
                .Replace(" ,", ",").Replace(" ;", ";");

            return new TranslatorResult(context, translationResult);
        }

        #endregion Public methods
    }
}