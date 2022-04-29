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
using JNogueira.Discord.Webhook.Client;

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

            IConfigurationSection WebHookChannels = _config.GetSection("WebHookChannels");
            IEnumerable<IConfigurationSection> WebHookLink = WebHookChannels.GetChildren();

            foreach (var hook in WebHookLink)
            {
                channelWebHook[hook.Key] = hook.Value;
            }
        }

        //[Command("Getm"), Alias("gm")]
        //public Task GetMessageAsString()
        //{
        //    var guild = Context.Guild;
        //    var message = Context.Message;
        //    var item = Context.Channel.GetMessageAsync(943768506892169236);
        //    return Console.Out.WriteLineAsync(item.ToString());
        //}

        [Command("Перенос"), Alias("sw")]
        [Summary("Дружелюбно переносит сообщение в другой канал")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        public async Task RedirectMessage([Remainder] string _channel)
        {
            var guild = Context.Guild;
            var attachments = Context.Message.Attachments;
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
            var textMessage = refMessage.Content;

            if (refMessage.Attachments.Count > 0)
            {
                foreach(var att in refMessage.Attachments)
                {
                    textMessage += $"{Environment.NewLine}{att.Url}";
                }
            }

            Discord.Webhook.DiscordWebhookClient DCW = new Discord.Webhook.DiscordWebhookClient(channelWebHook[channel.Name]);
            await DCW.SendMessageAsync(textMessage, false, null, refMessage.Author.Username, refMessage.Author.GetAvatarUrl());
            await Context.Message.DeleteAsync();
            await refMessage.DeleteAsync();
        }

        [Command("Red"), Alias("R")]
        [Summary("Добавит красную боковую линию к сообщению")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        public async Task RedText([Remainder] string msg)
        {
            await SendColorText(msg, Context, Context.Channel, DiscordColor.Red);
        }
        [Command("Green"), Alias("G")]
        [Summary("Добавит зелёную боковую линию к сообщению")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        public async Task GreenText([Remainder] string msg)
        {
            await SendColorText(msg, Context, Context.Channel, DiscordColor.Green);
        }

        [Command("Blue"), Alias("B")]
        [Summary("Добавит синюю боковую линию к сообщению")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageWebhooks)]
        public async Task BlueText([Remainder] string msg)
        {
            await SendColorText(msg, Context, Context.Channel, DiscordColor.Blue);
        }

        private async Task SendColorText(string msg, SocketCommandContext Context, IMessageChannel channel, DiscordColor color)
        {
            var client = new JNogueira.Discord.Webhook.Client.DiscordWebhookClient(channelWebHook[channel.Name]);
            DiscordMessage message = new DiscordMessage(
                                    username: Context.Message.Author.Username,
                                    avatarUrl: Context.Message.Author.GetAvatarUrl(),
                                    tts: false);
            if (msg.Length >= 255)
            {
                message = new DiscordMessage(
                        username: Context.Message.Author.Username,
                        avatarUrl: Context.Message.Author.GetAvatarUrl(),
                        tts: false,
                        embeds: new[]
                        {
                            new DiscordMessageEmbed(
                                color: (int)color,
                                description: msg)
                        });
            }
            else
            {
                message = new DiscordMessage(
                        username: Context.Message.Author.Username,
                        avatarUrl: Context.Message.Author.GetAvatarUrl(),
                        tts: false,
                        embeds: new[]
                        {
                            new DiscordMessageEmbed(
                                color: (int)color,
                                title: msg)
                        });
            }

            await client.SendToDiscord(message);
            await Context.Message.DeleteAsync();
        }
    }
}
