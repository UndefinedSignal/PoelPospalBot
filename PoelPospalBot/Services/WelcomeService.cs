using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoelPospalBot.Services
{
    public class WelcomeService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IConfigurationRoot _config;

        public WelcomeService(DiscordSocketClient discord, IConfigurationRoot config)
        {
            _config = config;
            _discord = discord;

            _discord.GuildMemberUpdated += GuildMemberUpdatedAsync;
            _discord.UserJoined += UserJoidGuild;
            _discord.UserLeft += UserLeftGuild;
        }

        private async Task GuildMemberUpdatedAsync(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
        {
            if(arg1.Value.IsBot || arg2.IsBot)
            {
                return;
            }
            ulong channelID = Convert.ToUInt64(_config["Channels:GuildeMemberUpdated"]);

            var channel = _discord.GetChannel(channelID) as IMessageChannel;
            var isNewNickname = !string.IsNullOrEmpty(arg2.Nickname) && !string.Equals(arg1.Value.Nickname, arg2.Nickname);
            var Nickname = isNewNickname ? arg2.Nickname : $"{arg1.Value.Nickname} -> {arg2.Nickname}";
            var isNewAvatar = arg1.Value.GetAvatarUrl() != arg2.GetAvatarUrl();
            if (isNewAvatar || isNewNickname)
            {
                var builder = new EmbedBuilder()
                {
                    Title = arg2.Username,
                    Description = arg2.Mention,
                    Color = Color.Green,
                    ThumbnailUrl = arg2.GetAvatarUrl()
                }
                .WithFooter(footer => footer.Text = arg2.Id.ToString())
                .WithCurrentTimestamp();

                await channel.SendMessageAsync($" ", false, builder.Build());
            }
            else
            {
                return;
            }
        }

        private async Task UserLeftGuild(SocketGuild arg1, SocketUser arg2)
        {
            ulong guildID = Convert.ToUInt64(_config["Guild:id"]);
            var guild = _discord.GetGuild(guildID);
            var channel = guild.GetChannel(637921143600185344) as IMessageChannel;

            var builder = new EmbedBuilder()
            {
                Title = $"{arg2.Username} покидает нас!",
                Color = Color.Magenta,
                Description = $"{arg2.Mention} до скорых встреч!",
                ThumbnailUrl = arg2.GetAvatarUrl()
            }
            .WithFooter($"ID: {arg2.Id}", arg2.GetAvatarUrl())
            .WithCurrentTimestamp();

            await channel.SendMessageAsync($"{arg2.Mention}", false, builder.Build());
        }

        private async Task UserJoidGuild(SocketGuildUser arg)
        {
            if (arg.IsBot)
            {
                return;
            }
            ulong guildID = Convert.ToUInt64(_config["Guild:id"]);
            var guild = _discord.GetGuild(guildID);
            var channel = guild.GetChannel(637921143600185344) as IMessageChannel;

            var builder = new EmbedBuilder()
            {
                Title = $"Привет {arg.Username}!",
                Color = Color.Magenta,
                Description = "Не забудь зайти и ознакомиться с правилами в канале <#637904093209427990> ;)",
                ThumbnailUrl = arg.GetAvatarUrl()
            }
            .WithCurrentTimestamp();

            var button = new ComponentBuilder()
                .WithButton(label: "Правила", customId: null, ButtonStyle.Link, Emote.Parse("<:catuwu:856531445854371850>"), "https://ptb.discord.com/channels/637904093209427988/637904093209427990/637930423737122816");
            await channel.SendMessageAsync($"{arg.Mention}", false, builder.Build(), null, null, null, button.Build());
            await arg.SendMessageAsync("https://cdn.discordapp.com/attachments/796283611972239380/858022267434303508/Goro_birbjima_Yakuza_shorts.mp4");
        }
    }
}
