using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatSaverSharp
{
    /// <summary>
    /// BeatSaver User
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonProperty("_id")]
        public string ID { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Fetch all Beatmaps uploaded by this user
        /// </summary>
        /// <param name="page">Optional Page Index</param>
        /// <returns></returns>
        public async Task<Page> Beatmaps(uint page = 0)
        {
            string pageURI = $"maps/{PageType.Uploader}/{ID}";
            string url = $"{pageURI}/{page}";

            Page p = await BeatSaver.FetchPaged(url);
            p.PageURI = pageURI;

            return p;
        }
    }
}
