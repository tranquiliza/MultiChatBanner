namespace MultiChatBanner
{
    public class TwitchClientSettings
    {
        public string TwitchUsername { get; }
        public string TwitchOAuth { get; }

        public TwitchClientSettings(string twitchUsername, string twitchOAuth)
        {
            TwitchUsername = twitchUsername;
            TwitchOAuth = twitchOAuth;
        }
        
    }
}