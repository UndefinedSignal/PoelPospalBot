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
    [Name("Разное для веселья")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;
        private IConfigurationRoot _config;

        public FunModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }

        [Command("Rick")]
        [Summary("Рикролльнет кнопочкой")]
        public async Task Spawn()
        {
            var builder = new ComponentBuilder()
                .WithButton(label: "Точно не РикРолл!", customId: null, ButtonStyle.Link, Emote.Parse("<:maddyking:856531445966831666>"), "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            await Context.Message.DeleteAsync();
            await ReplyAsync("WOWOWA WEEEYYYEEEE!", components: builder.Build());
        }
    }
}
