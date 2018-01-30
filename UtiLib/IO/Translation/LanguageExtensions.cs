using System;

namespace UtiLib.IO.Translation
{
    public static class LanguageExtensions
    {
        public static string ToShortForm(this Language language)
        {
            var langMap = SharedPreferences.LanguageMap;

            if (langMap.TryGetValue(language, out var mode))
            {
                return mode;
            }

            return string.Empty;
        }

        public static Language? ToLanguage(this string languageString)
        {
            if (Enum.TryParse<Language>(languageString, true, out var enumValue))
            {
                return enumValue;
            }

            return null;
        }
    }
}