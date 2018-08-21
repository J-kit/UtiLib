using System;
using System.Net;

//using Newtonsoft.Json;

namespace UtiLib.Net
{
    public class CookieWebClient : WebClient
    {
        private object _sharedLock;

        public CookieWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        public CookieWebClient() : base()
        {
        }

        public CookieContainer CookieContainer { get; set; } = new CookieContainer();
        public object SharedLock { get => _sharedLock ?? (_sharedLock = new object()); set => _sharedLock = value; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var r = base.GetWebRequest(address);
            if (r is HttpWebRequest request)
                request.CookieContainer = CookieContainer;

            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            if (r is HttpWebResponse response)
            {
                lock (SharedLock)
                    CookieContainer.Add(response.Cookies);
            }
        }
    }
}