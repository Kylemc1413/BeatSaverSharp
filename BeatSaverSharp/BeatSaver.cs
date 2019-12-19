using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;

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

        internal static async Task<Page> FetchPaged(string url, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync(url, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<Page>();
        }

        internal static async Task<Beatmap> FetchSingle(string url, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync(url, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<Beatmap>();
        }

        internal static async Task<Page> FetchMapsPage(string type, uint page = 0, IProgress<double> progress = null)
        {
            Page p = await FetchPaged($"maps/{type}/{page}", progress);
            p.PageURI = $"maps/{type}";

            return p;
        }

        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Latest(uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Latest, page, progress);
        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Hot(uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Hot, page, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Rating(uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Rating, page, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Downloads(uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Downloads, page, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Optional page number</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Plays(uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Plays, page, progress);

        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Key(string key, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Key}/{key}", progress);
        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Hash(string hash, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Hash}/{hash}", progress);

        internal static async Task<Page> FetchSearchPage(string searchType, string query, uint page = 0, IProgress<double> progress = null)
        {
            if (query == null) throw new ArgumentNullException("query");

            string encoded = HttpUtility.UrlEncode(query);
            string pageURI = $"search/{searchType}";

            string url = $"{pageURI}/{page}?q={encoded}";
            Page p = await FetchPaged(url, progress);

            p.Query = query;
            p.PageURI = pageURI;

            return p;
        }

        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional Page Index</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Search(string query, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Text, query, page, progress);

        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional Page Index</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> SearchAdvanced(string query, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Advanced, query, page, progress);

        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<User> User(string id, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync($"users/find/{id}", progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<User>();
        }
    }
}
