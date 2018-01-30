namespace UtiLib.IO.Translation
{
    public class TranslationContext
    {
        public Language SourceLanguage { get; private set; }
        public Language TargetLanguage { get; private set; }

        public string SourceShort => _sourceCache ?? (_sourceCache = SourceLanguage.ToShortForm());
        public string TargetShort => _targetCache ?? (_targetCache = TargetLanguage.ToShortForm());

        private string _sourceCache = null;
        private string _targetCache = null;

        public static TranslationContext Create(Language srcLanguage, Language dstLanguage)
        {
            return new TranslationContext { SourceLanguage = srcLanguage, TargetLanguage = dstLanguage };
        }
    }
}