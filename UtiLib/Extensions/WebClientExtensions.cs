using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UtiLib;
using UtiLib.Serialisation;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class WebExtensions
    {
        #region WebClient

        /// <summary>
        ///     Fills object with credible information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wc"></param>
        /// <returns></returns>
        public static T Prepare<T>(this T wc) where T : WebClient
        {
            wc.Proxy = null;
            wc.Encoding = Settings.DefaultEncoding;
            wc.Headers = new WebHeaderCollection
            {
                ["Cache-Control"] = "max-age=0",
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
                ["User-agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0",
                ["Accept-Language"] = "de,en-US;q=0.7,en;q=0.3"
            };
            return wc;
        }

        public static async Task<T> PostTaskAsync<T>(this WebClient wc, string url, NameValueCollection postCollection)
            where T : class
        {
            var tempResult = await wc.UploadValuesTaskAsync(url, "POST", postCollection);
            if (typeof(T) == typeof(byte[]))
                return tempResult as T;
            if (typeof(T) == typeof(string)) return tempResult.GetString() as T;
            throw new NotSupportedException($"Casting to {typeof(T).FullName} is not supported yet");
        }

        public static T DownloadXml<T>(this WebClient wc, string uri) where T : class
        {
            return wc.DownloadString(uri).ConvertResultXml<T>();
        }

        public static T DownloadJson<T>(this WebClient wc, string uri) where T : class
        {
            return wc.DownloadString(uri).ConvertResultJson<T>();
        }

        public static async Task<T> DownloadXmlAsync<T>(this WebClient wc, string uri) where T : class
        {
            return (await wc.DownloadStringTaskAsync(uri)).ConvertResultXml<T>();
        }

        public static async Task<T> DownloadJsonAsync<T>(this WebClient wc, string uri) where T : class
        {
            return (await wc.DownloadStringTaskAsync(uri)).ConvertResultJson<T>();
        }

        private static T ConvertResultJson<T>(this string input) where T : class
        {
            return input.ConvertResult<T>(Settings.JsonSerializer);
        }

        private static T ConvertResultXml<T>(this string input) where T : class
        {
            return input.ConvertResult<T>(Settings.XmlSerializer);
        }

        private static T ConvertResult<T>(this string input, ISerializer serializer) where T : class
        {
            if (typeof(T) == typeof(string)) return input as T;

            return serializer.DeserializeObject<T>(input.GetBytes());
        }

        #endregion WebClient

        public static bool TrySetValue(this CookieContainer cookieContainer, string site, string key, string value)
        {
            return cookieContainer.TrySetValue(new Uri(site), key, value);
        }

        public static bool TrySetValue(this CookieContainer cookieContainer, Uri site, string key, string value)
        {
            if (cookieContainer == null)
                return false;

            return cookieContainer.GetCookies(site).TrySetValue(key, value);
        }

        public static bool TrySetValue(this CookieCollection cookieCollection, string key, string value)
        {
            if (cookieCollection == null)
                return false;

            return cookieCollection[key].TrySetValue(value);
        }

        public static bool TrySetValue(this Cookie cookie, string value)
        {
            if (cookie == null)
                return false;

            cookie.Value = value;
            return true;
        }
    }
}