using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace PoelPospalBot.Modules
{
    [Name("Информация")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;
        private IConfigurationRoot _config;

        public InfoModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("Команды")]
        [Summary("Выводит список доступных команд")]
        public async Task CommandsInfo()
        {
            var builder = new EmbedBuilder()
            {
                Color = Color.DarkMagenta,
                Description = "Но это всё что у меня есть..."
            };
            foreach(var module in _service.Modules)
            {
                string description = null;
                foreach(var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if(result.IsSuccess)
                    {
                        description += $"{_config["prefix"]}{cmd.Aliases.First()} — {cmd.Summary}.\n";
                    }
                }

                if (!string.IsNullOrEmpty(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await Context.Message.DeleteAsync();
            await ReplyAsync("", false, builder.Build());
        }

        [Command("Сервер")]
        [Summary("Выдаёт информацию по серверу")]
        public async Task ServerInfo()
        {
            var guild = Context.Guild;
            await guild.DownloadUsersAsync();

            var online = guild.Users.Where(x => x.Status == UserStatus.Online);
            var textCh = guild.TextChannels.Count();

            var builder = new EmbedBuilder()
            {
                Color = Color.Red,
            };
            builder.AddField("Онлайн", $"На сервере: {guild.Users.Count} человек");
            builder.AddField("Каналы", $"Текстовых каналов всего: {textCh} шт.");
            builder.WithThumbnailUrl(guild.IconUrl);
            builder.WithCurrentTimestamp();

            await Context.Message.DeleteAsync();
            await ReplyAsync(null, false, builder.Build());
        }

        [Command("тесттест")]
        public async Task UserInfo()
        {

            await ReplyAsync($"Не реализовано, чё пыришься {Context.User.Mention} пидор?)"); ;
        }


    }
}