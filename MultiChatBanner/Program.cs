using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MultiChatBanner
{
    public static class Program
    {
        public static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            var settings = GetCredentialsFromUser(args);

            var twitchConnection = await ConnectToTwitchAsync(settings);

            await JoinChannels(twitchConnection);

            await BanUser(twitchConnection);
        }

        private static async Task BanUser(TwitchIntegration twitchConnection)
        {
            Console.Clear();
            Console.WriteLine("Input username that you want to ban!");
            var isInputtingUsernameToBan = true;

            while (isInputtingUsernameToBan)
            {
                if (!Console.KeyAvailable)
                    await Task.Delay(500);

                var input = Console.ReadLine();

                if (string.Equals("exit", input, StringComparison.InvariantCultureIgnoreCase))
                {
                    isInputtingUsernameToBan = false;
                    continue;
                }

                await twitchConnection.BanUser(input);

                Console.Clear();
                Console.WriteLine($"Finished banning {input}");
                Console.WriteLine();
                Console.WriteLine("Type exit and press enter to exit");
                Console.WriteLine("Input username that you want to ban!");
            }

            Console.WriteLine("Exited Banning");
        }

        private static async Task JoinChannels(TwitchIntegration twitchConnection)
        {
            Console.WriteLine("Please input names of channels you wish to ban for! (Press enter to submit)");
            Console.WriteLine("Input 'continue' or 'c' to continue");

            var isInputtingChannels = true;

            const string filePath = "channels.txt";
            if (File.Exists(filePath))
            {
                Console.WriteLine("channels.txt found, reading from file!");

                var fileContent = await File.ReadAllLinesAsync(filePath);
                foreach (var channelName in fileContent)
                {
                    twitchConnection.JoinChannel(channelName);
                    Console.WriteLine($"Joined {channelName}");
                }

                Console.WriteLine("Finished Joining channels! GET READY!");
                await Task.Delay(1000);

                return;
            }

            while (isInputtingChannels)
            {
                if (!Console.KeyAvailable)
                    await Task.Delay(500);

                var input = Console.ReadLine();

                if (string.Equals("continue", input, StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals("c", input, StringComparison.InvariantCultureIgnoreCase))
                {
                    isInputtingChannels = false;
                    continue;
                }

                twitchConnection.JoinChannel(input);
                Console.WriteLine($"Joined {input}");
            }
        }

        private static TwitchClientSettings GetCredentialsFromUser(string[] args)
        {
            var username = "TwitchDoBetter";

            Console.WriteLine("Please input your OAuth");
            string auth;
            if (args.Length == 1)
                auth = args[0];
            else
                auth = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Thank you!");

            return new TwitchClientSettings(username, auth);
        }

        private static async Task<TwitchIntegration> ConnectToTwitchAsync(TwitchClientSettings settings)
        {
            var twitchClient = new TwitchIntegration(settings);
            Console.WriteLine("Initializing...");
            twitchClient.Initialize();

            Console.WriteLine("Connecting...");
            await twitchClient.ConnectAsync();
            Console.Clear();

            Console.WriteLine("Connected Successfully!");
            return twitchClient;
        }
    }
}