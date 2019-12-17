using System;

namespace BeatSaverSharp
{
    internal static class PageType
    {
        public const string Latest = "latest";
        public const string Hot = "hot";
        public const string Rating = "rating";
        public const string Downloads = "downloads";
        public const string Plays = "plays";

        public const string Uploader = "uploader";
        public const string Search = "search";
    }

    internal static class SingleType
    {
        public const string Key = "detail";
        public const string Hash = "by-hash";
    }

    internal static class SearchType
    {
        public const string Text = "text";
        public const string Advanced = "advanced";
    }
}
