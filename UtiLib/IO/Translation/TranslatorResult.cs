using System.Web;

namespace UtiLib.IO.Translation
{
    public class TranslatorResult
    {
        internal TranslatorResult(TranslationContext ctx, string result)
        {
            Value = result;
            Context = ctx;
        }

        public TranslationContext Context { get; internal set; }
        public string Value { get; set; }

        public string TextToSpeechLink => $"{SharedPreferences.BaseUrl}translate_tts?ie=UTF-8&q={HttpUtility.UrlEncode(Value)}&tl={SharedPreferences.LanguageMap[Context.TargetLanguage]}&total=1&idx=0&textlen={Value.Length}&client=gtx";
    }
}