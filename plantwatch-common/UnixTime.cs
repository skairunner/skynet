using System;

namespace plantwatch
{
    public static class UnixTime
    {
        public static DateTimeOffset FromUnix(long unixtime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixtime);
        }

        public static long ToUnix(DateTimeOffset dt)
        {
            return dt.ToUniversalTime().ToUnixTimeSeconds();
        }
    }
}