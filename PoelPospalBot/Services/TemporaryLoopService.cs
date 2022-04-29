using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace PoelPospalBot.Services
{
    public class TemporaryLoopService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IConfigurationRoot _config;
        private Timer _loopTimer;

        public TemporaryLoopService(
            DiscordSocketClient discord,
            IConfigurationRoot config)
        {
            _discord = discord;
            _config = config;

            InitializeLoop();
        }

        private void InitializeLoop()
        {
            _loopTimer = new Timer(TimeSpan.FromSeconds(60).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            _loopTimer.Elapsed += _timer_ElapsedAsync;
            _loopTimer.Start();

        }

        private async void _timer_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
            var unixTimestamp = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            if (_config["Herotimer"] == null || _config["Herotimer"] == "")
            {
                _config.GetSection("Herotimer").Value = "0";
            }
            var nextDayUnixTimeStamp = ulong.Parse(_config["Herotimer"]) + 75400; //76400;
            if (nextDayUnixTimeStamp >= unixTimestamp)
            {
                return;
            }

            ulong guildID = Convert.ToUInt64(_config["Guild:id"]);
            var guild = _discord.GetGuild(guildID);

            await guild.DownloadUsersAsync();

            ulong roleID = Convert.ToUInt64(_config["HeroOfTheDayRoleID"]);

            ulong channelId = Convert.ToUInt64(_config["Guild:channel"]);
            var channel = _discord.GetChannel(channelId) as IMessageChannel;

            var role = guild.Roles.FirstOrDefault(x => x.Id == roleID);

            var takeUser = GetRandomUserFromGuild(guild);

            Console.WriteLine($"Выбираем героя! Общее количество юзеров {guild.Users.Count} из них мы выбрали {takeUser.Nickname}");

            foreach (var user in guild.Users)
            {
                if (user.Roles.Contains(role))
                {
                    Console.WriteLine($"У {user.Nickname} была роль. Теперь нет.");
                    await user.RemoveRoleAsync(role);
                }
            }

            var builder = new EmbedBuilder()
            {
                Title = "У нас новый герой!",
                Color = Color.Red,
                Description = $"{takeUser.Mention} ты избираешься на почётную роль! {role.Mention}",
            };
            builder.WithThumbnailUrl(takeUser.GetAvatarUrl());
            builder.WithCurrentTimestamp();

            _config.GetSection("Herotimer").Value = unixTimestamp.ToString();

            await takeUser.AddRoleAsync(role);
            await channel.SendMessageAsync(null, false, builder.Build());
        }

        private SocketGuildUser GetRandomUserFromGuild(SocketGuild guild)
        {
            var user = guild.Users.ElementAt((new Random().Next(1, guild.Users.Count)));
            if(user.IsBot)
            {
                return GetRandomUserFromGuild(guild);
            }
            return user;
        }
    }
}
