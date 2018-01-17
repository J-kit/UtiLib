using System.Net;
using System.Threading.Tasks;

using UtiLib;
using UtiLib.Serialisation;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class WebClientExtensions
    {
        public static T PrepareClient<T>(this T wc) where T : WebClient
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

        public static T DownloadXml<T>(this WebClient wc, string uri) where T : class
                => wc.DownloadString(uri).ConvertResultXml<T>();

        public static T DownloadJson<T>(this WebClient wc, string uri) where T : class
                => wc.DownloadString(uri).ConvertResultJson<T>();

        public static async Task<T> DownloadXmlAsync<T>(this WebClient wc, string uri) where T : class
                => (await wc.DownloadStringTaskAsync(uri)).ConvertResultXml<T>();

        public static async Task<T> DownloadJsonAsync<T>(this WebClient wc, string uri) where T : class
                => (await wc.DownloadStringTaskAsync(uri)).ConvertResultJson<T>();

        private static T ConvertResultJson<T>(this string input) where T : class
                => input.ConvertResult<T>(Settings.JsonSerializer);

        private static T ConvertResultXml<T>(this string input) where T : class
                => input.ConvertResult<T>(Settings.XmlSerializer);

        private static T ConvertResult<T>(this string input, ISerializer serializer) where T : class
        {
            if (typeof(T) == typeof(string))
            {
                return input as T;
            }

            return serializer.DeserializeObject<T>(input.GetBytes());
        }
    }
}

//Ugly Code?
//public static class WebClientExtensions
//{
//    public static T DownloadXml<T>(this WebClient wc, string uri) where T : class
//        => wc.DownloadString(uri).ConvertResult<T>(Settings.DefaultXmlSerializer);

//    public static T DownloadJson<T>(this WebClient wc, string uri) where T : class
//        => wc.DownloadString(uri).ConvertResult<T>(Settings.DefaultJsonSerializer);

//    public static async Task<T> DownloadXmlAsync<T>(this WebClient wc, string uri) where T : class
//        => await wc.DownloadTypeAsync<T>(uri, Settings.DefaultXmlSerializer);

//    public static async Task<T> DownloadJsonAsync<T>(this WebClient wc, string uri) where T : class
//        => await wc.DownloadTypeAsync<T>(uri, Settings.DefaultJsonSerializer);

//    private static async Task<T> DownloadTypeAsync<T>(this WebClient wc, string uri, ISerializer serializer) where T : class
//    {
//        var res = await wc.DownloadStringTaskAsync(uri);
//        return res.ConvertResult<T>(serializer);
//    }

//    private static T ConvertResult<T>(this string input, ISerializer serializer) where T : class
//    {
//        if (typeof(T) == typeof(string))
//        {
//            return input as T;
//        }

//        return serializer.DeserializeObject<T>(input.GetBytes());
//    }
//}