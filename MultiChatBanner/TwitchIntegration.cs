using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MultiChatBanner
{
    public class TwitchIntegration
    {
        private readonly TwitchClientSettings settings;
        private readonly TwitchClient client;

        public TwitchIntegration(TwitchClientSettings settings)
        {
            this.settings = settings;
            this.client = new TwitchClient();
        }

        public void Initialize()
        {
            var credentials = new ConnectionCredentials(settings.TwitchUsername, settings.TwitchOAuth);
            client.Initialize(credentials);
        }

        public void JoinChannel(string channelName)
        {
            var clientHasAlreadyJoined = client.JoinedChannels.Any(x =>
                string.Equals(x.Channel, channelName, StringComparison.OrdinalIgnoreCase));
            if (clientHasAlreadyJoined)
                return;

            client.JoinChannel(channelName);
        }

        private TaskCompletionSource<bool> connectionCompletionTask = new TaskCompletionSource<bool>();

        public async Task ConnectAsync()
        {
            client.OnConnected += TwitchClient_OnConnected;
            client.Connect();

            await connectionCompletionTask.Task.ConfigureAwait(false);
        }

        private void TwitchClient_OnConnected(object _, OnConnectedArgs e)
        {
            client.OnConnected -= TwitchClient_OnConnected;

            connectionCompletionTask.SetResult(true);
            connectionCompletionTask = new TaskCompletionSource<bool>();
        }

        public async Task BanUser(string input)
        {
            foreach (var joinedChannel in client.JoinedChannels)
            {
                var channelName = joinedChannel.Channel;

                input = input.Replace("/ban ", "");
                SendMessage(channelName, "/ban " + input);
                Console.WriteLine($"Banned {input} from {channelName}");
                Console.WriteLine("Banning in next channel, in 1 second!");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void SendMessage(string channelName, string message)
        {
            client.SendMessage(channelName, message);
        }
    }
}