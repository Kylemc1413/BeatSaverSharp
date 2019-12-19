using System;
using System.Net.Http;

namespace BeatSaverSharp.Exceptions
{
    /// <summary>
    /// Raised when encountering a Rate Limit HTTP Status
    /// </summary>
    public class RateLimitExceededException : Exception
    {
        /// <summary>
        /// Rate Limit Info
        /// </summary>
        public readonly RateLimitInfo RateLimit;

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        public RateLimitExceededException(RateLimitInfo info)
        {
            RateLimit = info;
        }

        /// <summary>
        /// </summary>
        /// <param name="resp"></param>
        public RateLimitExceededException(HttpResponseMessage resp)
        {
            RateLimitInfo? info = RateLimitInfo.FromHttp(resp);

            if (info == null) throw new Exception("Could not parse rate limit info");
            else RateLimit = (RateLimitInfo)info;
        }
    }
}
