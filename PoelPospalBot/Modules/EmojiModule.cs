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
    [Name("Эмодзи!")]
    public class EmojiModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;
        private IConfigurationRoot _config;

        public EmojiModule(CommandService service, IConfigurationRoot config)
        {
            _service = service;
            _config = config;
        }
        public async Task SetEmojiAsync(SocketCommandContext Context)
        {
            if (Context.Message == null) return;
            IEmote emote = Emote.Parse("<:maddyking:856531445966831666>");

            await Context.Message.AddReactionAsync(emote);
        }
    }
}
