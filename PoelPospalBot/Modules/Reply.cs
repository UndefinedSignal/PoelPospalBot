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
        public async Task Say([Remainder] string str)
        {
            await Context.Message.DeleteAsync();

            if(str != "")
            {
                await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} ПРИВЕТ ДРУЖИЩЕ! «{str}» Это так на тебя похоже!");
                return;
            }
            await ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} ПРИВЕТ ДРУЖИЩЕ!");
        }
    }
}