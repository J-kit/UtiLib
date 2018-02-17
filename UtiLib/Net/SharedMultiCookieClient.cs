using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace UtiLib.Net
{
    public class SharedCookieMultiClient
    {
        public CookieContainer Credentials { get; set; }
        private readonly object _sharedLock = new object();

        public SharedCookieMultiClient(CookieContainer credentials = null)
        {
            Credentials = credentials ?? new CookieContainer();
        }

        public SharedCookieMultiClient()
        {
            Credentials = new CookieContainer();
        }

        public async Task<T> PostTaskAsync<T>(string url, NameValueCollection postCollection) where T : class
        {
            using (var wc = NewCookieWebClient())
            {
                return await wc.PostTaskAsync<T>(url, postCollection);
            }
        }

        public async Task<string> DownloadStringTaskAsync(string url)
        {
            using (var wc = NewCookieWebClient())
            {
                return await wc.DownloadStringTaskAsync(url);
            }
        }

        public async Task<T> DownloadJsonAsync<T>(string url) where T : class
        {
            using (var wc = NewCookieWebClient())
            {
                return await wc.DownloadJsonAsync<T>(url);
            }
        }

        public async Task<T> DownloadXmlAsync<T>(string url) where T : class
        {
            using (var wc = NewCookieWebClient())
            {
                return await wc.DownloadXmlAsync<T>(url);
            }
        }

        private CookieWebClient NewCookieWebClient()
        {
            return new CookieWebClient(Credentials ?? new CookieContainer()) { SharedLock = _sharedLock }.Prepare();
        }
    }
}