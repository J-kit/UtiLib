using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using UtiLib.Net.Web;
using UtiLib.Serialisation;

namespace UtiLib.Net
{
    public class EasyCookieContainer
    {
        public EventHandler<Cookie> OnCookieAdded;

        public IEnumerable<Cookie> Cookies => _cookies;

        private List<Cookie> _cookies = new List<Cookie>();
        private CookieContainer _cookieContainer = new CookieContainer();

        public EasyCookieContainer Add(Cookie cookie)
        {
            _cookies.Add(cookie);
            _cookieContainer.Add(cookie);
            OnCookieAdded?.Invoke(this, cookie);
            return this;
        }

        public string GetCredentials(string url, string cookie = null)
        {
            return GetCookies(url, cookie).FirstOrDefault()?.Value;
        }

        public IEnumerable<Cookie> GetCookies(string url, string cookieName = null)
        {
            var query = _cookies.Where(x => x.Domain.Contains(url));
            if (!string.IsNullOrEmpty(cookieName))
            {
                query = query.Where(x => x.Name == cookieName);
            }
            return query;
        }

        public static implicit operator CookieContainer(EasyCookieContainer c) => c._cookieContainer;
    }

    public class SharedCookieMultiClient
    {
        public EasyCookieContainer Credentials { get; set; }
        private readonly object _sharedLock = new object();

        private HttpClientPool _clientPool;

        public SharedCookieMultiClient()
            : this(new EasyCookieContainer())
        {
        }

        public SharedCookieMultiClient(EasyCookieContainer credentials = null)
        {
            Credentials = credentials ?? new EasyCookieContainer();
            _clientPool = new HttpClientPool(credentials);
        }

        public async Task<T> PostTaskAsync<T>(string url, NameValueCollection postCollection) where T : class
        {
            //using (var wc = NewCookieWebClient())
            //{
            //    return await wc.PostTaskAsync<T>(url, postCollection);
            //}

            using (var wc = _clientPool.GetClient(url))
            {
                var httpResult = await wc.HttpClient.PostAsync(url, WebUtils.ToByteArrayContent(postCollection));
                if (!httpResult.IsSuccessStatusCode)
                {
                    throw new AggregateException($"Httpstatus code: {httpResult.StatusCode}");
                }

                if (typeof(T) == typeof(string))
                {
                    return await httpResult.Content.ReadAsStringAsync() as T;
                }

                if (typeof(T) == typeof(byte[]))
                {
                    return await httpResult.Content.ReadAsByteArrayAsync() as T;
                }
                throw new NotSupportedException($"Casting to {typeof(T).FullName} is not supported yet");
            }
        }

        public Task<string> DownloadStringTaskAsync(string url)
        {
            return DownloadAnyAsync<string>(url, (ISerializer)null);
        }

        public Task<T> DownloadXmlAsync<T>(string url)
            where T : class
        {
            return DownloadAnyAsync<T>(url, Settings.XmlSerializer);
        }

        public Task<T> DownloadJsonAsync<T>(string url)
            where T : class
        {
            return DownloadAnyAsync<T>(url, Settings.JsonSerializer);
        }

        public async Task<T> DownloadAnyAsync<T>(string url, ISerializer serializer)
            where T : class
        {
            try
            {
                using (var wc = _clientPool.GetClient(url))
                {
                    var httpResult = await wc.HttpClient.GetAsync(url);
                    if (!httpResult.IsSuccessStatusCode)
                    {
                        throw new AggregateException($"Status code: {httpResult.StatusCode}");
                    }
                    var httpContent = await httpResult.Content.ReadAsStringAsync();

                    return ConvertResult<T>(httpContent, serializer);
                }
            }
            catch (ArgumentException e)
            {
                // Console.WriteLine(e);
                CustomHttpClientHandler.ProtocolVersion = "1.1";
                return await DownloadAnyAsync<T>(url, serializer);
            }
        }

        private static T ConvertResult<T>(string input, ISerializer serializer) where T : class
        {
            if (typeof(T) == typeof(string))
                return input as T;

            return serializer.DeserializeObject<T>(input.GetBytes());
        }

        private CookieWebClient NewCookieWebClient()
        {
            return new CookieWebClient(Credentials ?? new CookieContainer()) { SharedLock = _sharedLock }.Prepare();
        }

        public async Task<string> DoStuff(string url)
        {
            var uri = new Uri(url);
            var handler = new CustomHttpClientHandler { UseCookies = true };
            var client = new HttpClient(handler);

            foreach (var cookie in Credentials.GetCookies(uri.Host))
            {
                handler.CookieContainer.Add(uri, cookie);
            }

            var result = await client.GetAsync(url);
            var res = await result.Content.ReadAsStringAsync();
            Debugger.Break();
            return res;
        }
    }

    internal class HttpClientPool
    {
        public EasyCookieContainer CookieContainer { get; }

        private ConcurrentDictionary<string, ConcurrentQueue<HttpClient>> _clientQueues;

        public HttpClientPool(EasyCookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer;
            _clientQueues = new ConcurrentDictionary<string, ConcurrentQueue<HttpClient>>();
        }

        public HttpClientPoolWrapper GetClient(string url)
        {
            var uri = new Uri(url);
            var queue = GetQueue(uri.Host);
            if (queue.IsEmpty ||
                !queue.TryDequeue(out var client))
            {
                var handler = new CustomHttpClientHandler { UseCookies = true };
                client = new HttpClient(handler);

                foreach (var cookie in CookieContainer.GetCookies(uri.Host))
                {
                    handler.CookieContainer.Add(uri, cookie);
                }

                CookieContainer.OnCookieAdded += (obj, cc) =>
                {
                    handler?.CookieContainer?.Add(new Uri($"https://{cc.Domain}"), cc);
                    handler?.CookieContainer?.Add(new Uri($"http://{cc.Domain}"), cc);
                };
                Console.WriteLine($"Http Pool: Generated a new instance of httpclient");
            }

            return new __HttpClientPoolWrapper(client, uri, this);
        }

        public void ReintegrateClient(HttpClientPoolWrapper client)
        {
            if (client is __HttpClientPoolWrapper wrapperClient)
            {
                GetQueue(wrapperClient.TargetUri.Host).Enqueue(wrapperClient.HttpClient);
            }
        }

        private bool TryGetClient(Uri uri, out HttpClient client)
        {
            var host = uri.Host;
            if (_clientQueues.TryGetValue(host, out var queue))
            {
                return queue.TryDequeue(out client);
            }

            _clientQueues[host] = new ConcurrentQueue<HttpClient>();
            client = null;

            return false;
        }

        private ConcurrentQueue<HttpClient> GetQueue(string url)
        {
            if (!_clientQueues.TryGetValue(url, out var queue))
            {
                queue = new ConcurrentQueue<HttpClient>();
                _clientQueues[url] = queue;
            }

            return queue;
        }

        // ReSharper disable once InconsistentNaming
        private class __HttpClientPoolWrapper : HttpClientPoolWrapper
        {
            private HttpClientPool _parent;
            public Uri TargetUri { get; }

            public __HttpClientPoolWrapper(HttpClient client, Uri targetUri, HttpClientPool parent)
            {
                TargetUri = targetUri;
                base.HttpClient = client;
                _parent = parent;
            }

            public override void Dispose()
            {
                _parent.ReintegrateClient(this);
                base.Dispose();
            }
        }
    }

    public abstract class HttpClientPoolWrapper : IDisposable
    {
        public HttpClient HttpClient { get; protected set; }

        public virtual void Dispose()
        {
        }
    }

    public class CustomHttpClientHandler : HttpClientHandler
    {
        public static string ProtocolVersion { get; set; } = "2.0";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                request.Version = new Version(ProtocolVersion);
                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
