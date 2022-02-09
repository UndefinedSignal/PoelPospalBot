using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoelPospalBot.Modules
{
    [Name("Информация")]
    public class Info : ModuleBase<SocketCommandContext>
    {
        [Command("Инфо")]
        public async Task ServerInfo()
        {
            var guild = Context.Guild;
            await guild.DownloadUsersAsync();

            var online = guild.Users.Where(x => x.Status == UserStatus.Online);
            var bots = guild.Users.Where(x => x.IsBot);
            var textCh = guild.TextChannels.Count();

            var builder = new EmbedBuilder()
            {
                Color = Color.Red,
            };
            builder.AddField("Online", $"На сервере: {guild.Users.Count} человек");
            builder.AddField("Bots", $"Из них: {bots} ботов");
            builder.AddField("TextChannels", $"Текстовых каналов всего: {textCh} шт.");
            builder.WithThumbnailUrl(guild.IconUrl);
            builder.WithCurrentTimestamp();

            await ReplyAsync(null, false, builder.Build());
        }

        [Command("Ricky")]
        public async Task Spawn()
        {
            var builder = new ComponentBuilder()
                .WithButton(label: "Link", customId: null, ButtonStyle.Link, Emote.Parse("<:maddyking:856531445966831666>"), "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            await Context.Message.DeleteAsync();
            await ReplyAsync("WOWOWA WEEEYYYEEEE!", components: builder.Build());
        }
    }
}