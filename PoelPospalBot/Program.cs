using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PoelPospalBot
{
    class Program
    {
        private DiscordSocketClient _client;

        public static Task Main(string[] args) => new Program().Start();

        private async Task Start()
        {
            _client = new DiscordSocketClient();

            _client.Log += Logger;

            var token = LoadToken();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Logger(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        private string LoadToken()
        {
            return System.IO.File.ReadAllText(@"..\..\..\Config\token.txt");
        }
    }
}
