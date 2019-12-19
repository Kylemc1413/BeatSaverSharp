using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatSaverSharp
{
    internal class Http
    {
        private static bool headersInit = false;
        internal static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri($"{BeatSaver.BaseURL}/api/"),
            Timeout = TimeSpan.FromSeconds(30),
        };

        internal static JsonSerializer Serializer = new JsonSerializer();

        private static void InitHeaders()
        {
            if (headersInit == true) return;
            headersInit = true;

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Client.DefaultRequestHeaders.Add("User-Agent", $"BeatSaver.Net/{version}");
        }

        internal static async Task<HttpResponse> GetAsync(string url, IProgress<double> progress = null)
        {
            InitHeaders();

            HttpResponseMessage resp = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            using (MemoryStream ms = new MemoryStream())
            using (Stream s = await resp.Content.ReadAsStreamAsync())
            {
                byte[] buffer = new byte[1 << 13];
                int bytesRead;

                long? contentLength = resp.Content.Headers.ContentLength;
                long totalRead = 0;
                progress?.Report(0);

                while ((bytesRead = await s.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (contentLength != null)
                    {
                        double prog = (double)totalRead / (double)contentLength;
                        progress?.Report(prog);
                    }

                    await ms.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                }

                progress?.Report(1);
                byte[] bytes = ms.ToArray();

                return new HttpResponse(resp, bytes);
            }
        }
    }

    internal class HttpResponse
    {
        public readonly HttpStatusCode StatusCode;
        public readonly string ReasonPhrase;
        public readonly HttpResponseHeaders Headers;
        public readonly HttpRequestMessage RequestMessage;
        public readonly bool IsSuccessStatusCode;

        private readonly byte[] _body;

        internal HttpResponse(HttpResponseMessage resp, byte[] body)
        {
            StatusCode = resp.StatusCode;
            ReasonPhrase = resp.ReasonPhrase;
            Headers = resp.Headers;
            RequestMessage = resp.RequestMessage;
            IsSuccessStatusCode = resp.IsSuccessStatusCode;

            _body = body;
        }

        public byte[] Bytes() => _body;
        public string String() => Encoding.UTF8.GetString(_body);
        public T JSON<T>()
        {
            string body = String();

            using (StringReader sr = new StringReader(body))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                return Http.Serializer.Deserialize<T>(reader);
            }
        }
    }
}
