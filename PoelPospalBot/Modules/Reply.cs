using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;


namespace PoelPospalBot.Modules
{
    [Name("FuckOff")]
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        [Command("привет")]
        public Task Say()
            => ReplyAsync($"{MentionUtils.MentionUser(Context.User.Id)} ПРИВЕТ ДРУЖИЩЕ!");
    }
}