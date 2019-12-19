using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.IO;

namespace BeatSaverSharp
{
    /// <summary>
    /// Beat Saver API Methods
    /// </summary>
    public static class BeatSaver
    {
        /// <summary>
        /// BeatSaver Base URL
        /// </summary>
        public const string BaseURL = "https://beatsaver.com";

        internal static JsonSerializer Serializer = new JsonSerializer();

        private static bool headersInit = false;
        internal static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri($"{BaseURL}/api/"),
        };

        private static void InitHeaders()
        {
            if (headersInit == true) return;
            headersInit = true;

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Client.DefaultRequestHeaders.Add("User-Agent", $"BeatSaver.Net/{version}");
        }

        internal static async Task<Page> FetchPaged(string url)
        {
            InitHeaders();

            var resp = await Client.GetAsync(url).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            using (Stream s = await resp.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Serializer.Deserialize<Page>(reader);
            }
        }

        internal static async Task<Beatmap> FetchSingle(string url)
        {
            InitHeaders();

            var resp = await Client.GetAsync(url).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            using (Stream s = await resp.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Serializer.Deserialize<Beatmap>(reader);
            }
        }

        internal static async Task<Page> FetchMapsPage(string type, uint page = 0)
        {
            Page p = await FetchPaged($"maps/{type}/{page}");
            p.PageURI = $"maps/{type}";

            return p;
        }

        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <returns></returns>
        public static async Task<Page> Latest(uint page = 0) => await FetchMapsPage(PageType.Latest, page);
        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <returns></returns>
        public static async Task<Page> Hot(uint page = 0) => await FetchMapsPage(PageType.Hot, page);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <returns></returns>
        public static async Task<Page> Rating(uint page = 0) => await FetchMapsPage(PageType.Rating, page);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <returns></returns>
        public static async Task<Page> Downloads(uint page = 0) => await FetchMapsPage(PageType.Downloads, page);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <returns></returns>
        public static async Task<Page> Plays(uint page = 0) => await FetchMapsPage(PageType.Plays, page);

        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <returns></returns>
        public static async Task<Beatmap> Key(string key) => await FetchSingle($"maps/{SingleType.Key}/{key}");
        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <returns></returns>
        public static async Task<Beatmap> Hash(string hash) => await FetchSingle($"maps/{SingleType.Hash}/{hash}");

        internal static async Task<Page> FetchSearchPage(string searchType, string query, uint page = 0)
        {
            if (query == null) throw new ArgumentNullException("query");

            string encoded = HttpUtility.UrlEncode(query);
            string pageURI = $"search/{searchType}";

            string url = $"{pageURI}/{page}?q={encoded}";
            Page p = await FetchPaged(url);

            p.Query = query;
            p.PageURI = pageURI;

            return p;
        }

        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional Page Index</param>
        /// <returns></returns>
        public static async Task<Page> Search(string query, uint page = 0) => await FetchSearchPage(SearchType.Text, query, page);

        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional Page Index</param>
        /// <returns></returns>
        public static async Task<Page> SearchAdvanced(string query, uint page = 0) => await FetchSearchPage(SearchType.Advanced, query, page);

        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        public static async Task<User> User(string id)
        {
            InitHeaders();

            var resp = await Client.GetAsync($"users/find/{id}").ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            using (Stream s = await resp.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Serializer.Deserialize<User>(reader);
            }
        }
    }
}
