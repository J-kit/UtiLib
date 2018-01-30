namespace UtiLib.IO.Translation
{
    public class TranslationContext
    {
        public Language SourceLanguage { get; set; }
        public Language DestinationLanguage { get; set; }

        public static TranslationContext Create(Language srcLanguage, Language dstLanguage)
        {
            return new TranslationContext { SourceLanguage = srcLanguage, DestinationLanguage = dstLanguage };
        }
    }
}