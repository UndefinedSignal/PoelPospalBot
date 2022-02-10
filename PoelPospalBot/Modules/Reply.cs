using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;


namespace PoelPospalBot.Modules
{
    [Name("FuckOff"), Summary("Дружелюбное приветствие")]
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        [Command("привет")]
        public async Task Say()
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} ПРИВЕТ ДРУЖИЩЕ!");
        }

        [Command("привет")]
        public async Task Say(string str)
        {
            await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} ПРИВЕТ ДРУЖИЩЕ! «{str}» Это так на тебя похоже!");
        }
    }
}