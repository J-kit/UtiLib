using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using UtiLib;

// ReSharper disable once CheckNamespace
namespace System
{
    public enum StringEncoding
    {
        Default,
        Hexadecimal,
        FormattedByte,
        Ascii,
        Utf8
    }

    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string input)
        {
            var ss = new SecureString();
            foreach (var @char in input)
            {
                ss.AppendChar(@char);
            }

            return ss;
        }

        public static string JoinStrings(this IEnumerable<string> input, string seperator)
        {
            return string.Join(seperator, input);
        }

        public static bool StartsWith(this string input, params string[] args)
        {
            return args.Any(input.StartsWith);
        }

        /// <summary>
        /// Synonyme for string.GetString(StringEncoding.Hexadecimal);
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] input)
            => input.GetString(StringEncoding.Hexadecimal);

        /// <summary>
        /// Synonyme for string.GetString(StringEncoding.Hexadecimal);
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetHexString(this byte[] input)
            => input.GetString(StringEncoding.Hexadecimal);

        public static string GetString(this byte[] input, StringEncoding encoding = StringEncoding.Utf8)
        {
            //Select(x => x.ToString()).JoinStrings(" ")
            switch (encoding)
            {
                case StringEncoding.Hexadecimal:
                    return string.Concat(input.Select(b => b.ToString("x2")).ToArray());

                case StringEncoding.FormattedByte:
                    return input.Select(x => x.ToString()).JoinStrings(" ");

                case StringEncoding.Ascii:
                    return Encoding.ASCII.GetString(input);

                case StringEncoding.Utf8:
                    return Encoding.UTF8.GetString(input);

                case StringEncoding.Default:
                    return Encoding.Default.GetString(input);

                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }
        }

        public static byte[] GetBytes(this string input, StringEncoding encoding = StringEncoding.Utf8)
        {
            switch (encoding)
            {
                case StringEncoding.Ascii:
                    return Encoding.ASCII.GetBytes(input);

                case StringEncoding.Utf8:
                    return Encoding.UTF8.GetBytes(input);

                case StringEncoding.Default:
                    return Encoding.Default.GetBytes(input);

                case StringEncoding.Hexadecimal:
                    throw new NotImplementedException("Not implemented yet");
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }
        }

        public static string FormatEx(this string input, params object[] args)
                => string.Format(input, args);

        public static string[] Split(this string s, string separator)
                => s.Split(new[] { separator }, StringSplitOptions.None);

        public static string UrlEncode(this string input)
        {
            return HttpUtility.UrlEncode(input, Settings.DefaultEncoding);
        }

        public static string UrlDecode(this string input)
        {
            return HttpUtility.UrlDecode(input, Settings.DefaultEncoding);
        }

        public static string HtmlEncode(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        public static string HtmlDecode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        /// <summary>
        /// Parses a string into an Enum
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <param name="ignorecase">Ignore the case of the string being parsed</param>
        /// <returns>The Enum corresponding to the stringExtensions</returns>
        public static T ToEnum<T>(this string value, bool ignorecase)
        {
            if (value == null)
                throw new ArgumentNullException("Value");

            value = value.Trim();

            if (value.Length == 0)
                throw new ArgumentNullException("Must specify valid information for parsing in the string.", "value");

            var t = typeof(T);
            if (!t.IsEnum)
                throw new ArgumentException("Type provided must be an Enum.", "T");

            return (T)Enum.Parse(t, value, ignorecase);
        }

        /// <summary>
        /// Determines whether the string is not null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        /// <summary>
        ///  Determines whether it is a valid URL.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string text)
        {
            return ValidUrlRegex.IsMatch(text);
        }

        /// <summary>
        /// Determines whether it is a valid email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string email)
        {
            return ValidMailRegex.IsMatch(email);
        }

        private static readonly Regex ValidUrlRegex = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        private static readonly Regex ValidMailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    }
}