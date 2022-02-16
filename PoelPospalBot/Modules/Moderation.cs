using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Webhook;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace PoelPospalBot.Modules
{
    [Name("Модерация"), Summary("Технологии дружелюбных соседей")]
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        private Dictionary<string, string> channelWebHook = new Dictionary<string, string>();

        public Moderation(IConfigurationRoot config)
        {
            _config = config;

            IConfigurationSection WebHookChannels = _config.GetSection("HookChannels");
            IEnumerable<IConfigurationSection> WebHookLink = WebHookChannels.GetChildren();

            foreach (var hook in WebHookLink)
            {
                channelWebHook[hook.Key] = hook.Value;
            }
        }

        [Command("Перенос"), Alias("sw")]
        [Summary("Дружелюбно переносит сообщение в другой канал")]
        public async Task RedirectMessage([Remainder]string _channel)
        {
            var guild = Context.Guild;
            _channel = _channel.Trim(new char[] { '#', ' ', '<', '>' });

            IMessageChannel channel;
            if (_channel.All(char.IsDigit))
            {
                channel = guild.GetChannel(Convert.ToUInt64(_channel)) as IMessageChannel;
            }
            else
            {
                channel = guild.Channels.FirstOrDefault(x => x.Name.ToString() == _channel) as IMessageChannel;
            }
            var refMessage = Context.Message.ReferencedMessage;

            DiscordWebhookClient DCW = new DiscordWebhookClient(channelWebHook[channel.Name]);
            await DCW.SendMessageAsync(refMessage.Content, false, null, refMessage.Author.Username, refMessage.Author.GetAvatarUrl());
            await Context.Message.DeleteAsync();
            await refMessage.DeleteAsync();
        }
    }
}
