﻿using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeatSaver.Exceptions;

namespace BeatSaverSharp
{
    /// <summary>
    /// BeatSaver Beatmap
    /// </summary>
    public sealed class Beatmap
    {
        #region Static Methods
        /// <summary>
        /// Fetch a Beatmap by Key
        /// </summary>
        /// <param name="key">Hex Key</param>
        /// <returns></returns>
        public static async Task<Beatmap> FromKey(string key) => await BeatSaver.Key(key);
        /// <summary>
        /// Fetch a Beatmap by Hash
        /// </summary>
        /// <param name="hash">SHA1 Hash</param>
        /// <returns></returns>
        public static async Task<Beatmap> FromHash(string hash) => await BeatSaver.Hash(hash);
        #endregion

        #region JSON Properties
        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonProperty("_id")]
        public string ID { get; set; }

        /// <summary>
        /// Hex Key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Multiline description. Can be null.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// User who uploaded this beatmap
        /// </summary>
        [JsonProperty("uploader")]
        public User Uploader { get; set; }

        /// <summary>
        /// Metadata for the Beatmap .dat file
        /// </summary>
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        /// <summary>
        /// Direct Download URL. Skips the download counter.
        /// </summary>
        [JsonProperty("directDownload")]
        public string DirectDownload { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("downloadURL")]
        public string DownloadURL { get; set; }

        /// <summary>
        /// URL for the Cover Art
        /// </summary>
        [JsonProperty("coverURL")]
        public string CoverURL { get; set; }

        /// <summary>
        /// File name for the Cover Art
        /// </summary>
        [JsonIgnore]
        public string CoverFilename
        {
            get => System.IO.Path.GetFileName(CoverURL);
        }

        /// <summary>
        /// SHA1 Hash
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Fetch latest values
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            Beatmap b = await FromHash(Hash);

            Name = b.Name;
            Description = b.Description;
            Stats = b.Stats;
        }

        /// <summary>
        /// Fetch latest stats
        /// </summary>
        /// <returns></returns>
        public async Task RefreshStats()
        {
            Beatmap b = await FromHash(Hash);
            Stats = b.Stats;
        }

        private enum VoteDirection : short
        {
            Up = 1,
            Down = -1,
        }

        private struct VotePayload
        {
            [JsonProperty("steamID")]
            public string SteamID { get; set; }

            [JsonProperty("ticket")]
            public string Ticket { get; set; }

            [JsonProperty("direction")]
            public string Direction { get; set; }

            public VotePayload(VoteDirection direction, string steamID, byte[] authTicket)
            {
                SteamID = steamID;
                Ticket = string.Concat(Array.ConvertAll(authTicket, x => x.ToString("X2")));
                Direction = ((short)direction).ToString();
            }

            public VotePayload(VoteDirection direction, string steamID, string authTicket)
            {
                SteamID = steamID;
                Ticket = authTicket;
                Direction = ((short)direction).ToString();
            }
        }

        private async Task<bool> Vote(VoteDirection direction, string steamID, byte[] authTicket)
        {
            VotePayload payload = new VotePayload(direction, steamID, authTicket);
            return await Vote(payload);
        }

        private async Task<bool> Vote(VoteDirection direction, string steamID, string authTicket)
        {
            VotePayload payload = new VotePayload(direction, steamID, authTicket);
            return await Vote(payload);
        }

        private async Task<bool> Vote(VotePayload payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await BeatSaver.Client.PostAsync($"vote/steam/{Key}", content);
            if (resp.IsSuccessStatusCode)
            {
                using (Stream s = await resp.Content.ReadAsStreamAsync())
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Beatmap updated = BeatSaver.Serializer.Deserialize<Beatmap>(reader);
                    Stats = updated.Stats;

                    return true;
                }
            }

            RestError error;
            using (Stream s = await resp.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                error = BeatSaver.Serializer.Deserialize<RestError>(reader);
            }

            if (error.Identifier == "ERR_INVALID_STEAM_ID") throw new InvalidSteamIDException(payload.SteamID);
            if (error.Identifier == "ERR_STEAM_ID_MISMATCH") throw new InvalidSteamIDException(payload.SteamID);
            if (error.Identifier == "ERR_INVALID_TICKET") throw new InvalidTicketException();
            if (error.Identifier == "ERR_BAD_TICKET") throw new InvalidTicketException();

            return false;
        }

        /// <summary>
        /// Submit an Upvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket</param>
        /// <returns></returns>
        public async Task<bool> VoteUp(string steamID, byte[] authTicket) => await Vote(VoteDirection.Up, steamID, authTicket);
        /// <summary>
        /// Submit an Upvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket (Hex String)</param>
        /// <returns></returns>
        public async Task<bool> VoteUp(string steamID, string authTicket) => await Vote(VoteDirection.Up, steamID, authTicket);

        /// <summary>
        /// Submit a Downvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket</param>
        /// <returns></returns>
        public async Task<bool> VoteDown(string steamID, byte[] authTicket) => await Vote(VoteDirection.Down, steamID, authTicket);
        /// <summary>
        /// Submit a Downvote for this Beatmap
        /// </summary>
        /// <param name="steamID">Steam ID to submit as</param>
        /// <param name="authTicket">Steam Authentication Ticket (Hex String)</param>
        /// <returns></returns>
        public async Task<bool> VoteDown(string steamID, string authTicket) => await Vote(VoteDirection.Down, steamID, authTicket);

        /// <summary>
        /// Download the Beatmap Zip as a byte array
        /// </summary>
        /// <param name="direct">If true, will skip counting the download request</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadZip(bool direct = false)
        {
            string url = direct ? DirectDownload : DownloadURL;
            var resp = await BeatSaver.Client.GetAsync($"{BeatSaver.BaseURL}{url}");

            return await resp.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Fetch the Beatmap's Cover Image as a byte array
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> FetchCoverImage()
        {
            var resp = await BeatSaver.Client.GetAsync($"{BeatSaver.BaseURL}{CoverURL}");
            return await resp.Content.ReadAsByteArrayAsync();
        }
        #endregion
    }
}