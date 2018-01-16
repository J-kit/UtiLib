using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace System
{
    public enum StringEncoding
    {
        Default,
        Hexadecimal,
        Ascii,
        Utf8
    }

    public static class StringExtensions
    {
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
            switch (encoding)
            {
                case StringEncoding.Hexadecimal:
                    return string.Concat(input.Select(b => b.ToString("x2")).ToArray());

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
                => String.Format(input, args);

        public static string[] Split(this string s, string separator)
                => s.Split(new[] { separator }, StringSplitOptions.None);

        public static string UrlEncode(this string input)
        {
            return HttpUtility.UrlEncode(input);
        }
    }
}