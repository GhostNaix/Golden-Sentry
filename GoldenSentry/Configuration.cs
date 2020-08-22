using System;
using System.Collections.Generic;

namespace GoldenSentry
{
    public class Configuration
    {
        public string AuthToken { get; set; } = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        public char prefix { get; set; } = '$';
        public bool StealthMode { get; set; } = false;
        public bool Overwatch { get; set; } = false;
        public bool LongWatch { get; set; } = false;
        public Int32 ScanInterval { get; set; } = 30;
        public HashSet<ulong> AuthorizedUserIDs { get; set; } = new HashSet<ulong>() { 349443731222560768 };
        public HashSet<ulong> Blacklist { get; set; } = new HashSet<ulong>();
        public HashSet<ulong> LoggingChannels { get; set; } = new HashSet<ulong>();
    }
}
