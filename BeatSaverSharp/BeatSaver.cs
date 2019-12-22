using System;
using System.Net;
using System.Threading;
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

        internal static async Task<Page> FetchPaged(string url, string userAgent, CancellationToken token, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync(url, userAgent, token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<Page>();
        }

        internal static async Task<Beatmap> FetchSingle(string url, string userAgent, CancellationToken token, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync(url, userAgent, token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<Beatmap>();
        }

        internal static async Task<Page> FetchMapsPage(string type, uint page, string userAgent, CancellationToken token, IProgress<double> progress = null)
        {
            Page p = await FetchPaged($"maps/{type}/{page}",userAgent, token, progress);
            p.PageURI = $"maps/{type}";

            return p;
        }

        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Latest(string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Latest, page, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a page of Latest beatmaps
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Latest(string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchMapsPage(PageType.Latest, page, userAgent, token, progress);

        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Hot(string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Hot, page, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a page of Hot beatmaps
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Hot(string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchMapsPage(PageType.Hot, page, userAgent, token, progress);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Rating(string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Rating, page, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their Rating
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Rating(string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchMapsPage(PageType.Rating, page, userAgent, token, progress);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Downloads(string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Downloads, page, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their download count
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Downloads(string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchMapsPage(PageType.Downloads, page, userAgent, token, progress);

        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Plays(string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchMapsPage(PageType.Plays, page, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a page of beatmaps ordered by their play count
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Plays(string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchMapsPage(PageType.Plays, page, userAgent, token, progress);

        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Key(string userAgent, string key, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Key}/{key}", userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Key(string userAgent, string key, CancellationToken token, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Key}/{key}", userAgent, token, progress);

        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Hash(string userAgent, string hash, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Hash}/{hash}", userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Beatmap> Hash(string userAgent, string hash, CancellationToken token, IProgress<double> progress = null) => await FetchSingle($"maps/{SingleType.Hash}/{hash}", userAgent, token, progress);

        internal static async Task<Page> FetchSearchPage(string searchType, string userAgent, string query, uint page, CancellationToken token, IProgress<double> progress = null)
        {
            if (query == null) throw new ArgumentNullException("query");

            string encoded = HttpUtility.UrlEncode(query);
            string pageURI = $"search/{searchType}";

            string url = $"{pageURI}/{page}?q={encoded}";
            Page p = await FetchPaged(url, userAgent, token, progress);

            p.Query = query;
            p.PageURI = pageURI;

            return p;
        }

        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Search(string query, string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Text, query, userAgent, page, CancellationToken.None, progress);
        /// <summary>
        /// Text Search
        /// </summary>
        /// <param name="query">Text Query</param>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> Search(string query, string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Text, query, userAgent, page, token, progress);

        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Optional page index (defaults to 0)</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> SearchAdvanced(string query, string userAgent, uint page = 0, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Advanced, query, userAgent, page, CancellationToken.None, progress);
        /// <summary>
        /// Advanced Lucene Search
        /// </summary>
        /// <param name="query">Lucene Query</param>
        /// <param name="page">Page index</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<Page> SearchAdvanced(string query, string userAgent, uint page, CancellationToken token, IProgress<double> progress = null) => await FetchSearchPage(SearchType.Advanced, query, userAgent, page, token, progress);

        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<User> User(string id, string userAgent, IProgress<double> progress = null) => await User(id, userAgent, CancellationToken.None, progress);
        /// <summary>
        /// Fetch a User by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="progress">Optional progress reporter</param>
        /// <returns></returns>
        public static async Task<User> User(string id, string userAgent, CancellationToken token, IProgress<double> progress = null)
        {
            var resp = await Http.GetAsync($"users/find/{id}", userAgent, token, progress).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            return resp.JSON<User>();
        }
    }
}
