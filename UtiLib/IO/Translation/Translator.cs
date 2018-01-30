// Copyright (c) 2015 Ravi Bhavnani
// License: Code Project Open License
// http://www.codeproject.com/info/cpol10.aspx

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.IO.Translation
{
    /// <summary>
    ///     Translates text using Google's online language tools.
    /// </summary>
    public class Translator
    {
        /// <summary>
        ///     Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="context"></param>
        /// <returns>The translation.</returns>
        public static TranslatorResult Translate(string sourceText, TranslationContext context)
        {
            var url = GenerateTranslateUrl(sourceText, context);
            var serverResponse = DownloadString(url);

            return ParseResult(serverResponse, context);
        }

        /// <summary>
        ///     Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="context"></param>
        /// <returns>The translation.</returns>
        public static async Task<TranslatorResult> TranslateAsync(string sourceText, TranslationContext context)
        {
            var url = GenerateTranslateUrl(sourceText, context);
            var serverResponse = await DownloadStringTaskAsync(url);

            return ParseResult(serverResponse, context);
        }

        private static string GenerateTranslateUrl(string sourceText, TranslationContext context)
        {
            return
                $"{SharedPreferences.BaseUrl}translate_a/single?client=gtx&sl={context.SourceShort}&tl={context.TargetShort}&dt=t&q={sourceText.UrlEncode()}";
        }

        private static string DownloadString(string url)
        {
            using (var wc = new WebClient().PrepareClient())
            {
                return wc.DownloadString(url);
            }
        }

        private static async Task<string> DownloadStringTaskAsync(string url)
        {
            using (var wc = new WebClient().PrepareClient())
            {
                return await wc.DownloadStringTaskAsync(url);
            }
        }

        private static TranslatorResult ParseResult(string serverResponse, TranslationContext context)
        {
            var index = serverResponse.IndexOf($",,\"{context.TargetShort}\"", StringComparison.Ordinal);
            var translationResult = index == -1 ? TranslateWords(serverResponse) : TranslatePhrases(index, serverResponse);

            // Fix up translation
            translationResult = translationResult.Trim()
                .Replace(" .", ".").Replace(" ?", "?").Replace(" !", "!")
                .Replace(" ,", ",").Replace(" ;", ";");

            return new TranslatorResult(context, translationResult);
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
            // ReSharper disable once JoinDeclarationAndInitializer
            string[] phrases;
            var sb = new StringBuilder();

            // Translation of phrase
            text = text.Substring(0, index)
                .Replace("],[", ",")
                .Replace("]", string.Empty)
                .Replace("[", string.Empty)
                .Replace("\",\"", "\"");

            // Get translated phrases
            phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}