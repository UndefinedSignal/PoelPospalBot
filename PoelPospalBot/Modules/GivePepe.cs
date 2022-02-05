using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PoelPospalBot.Modules
{
    [Name("Случайный Пепе")]
    public class PepeModule : ModuleBase<SocketCommandContext>
    {
        [Command("Пепе"), Alias("P")]
        public async Task PepeAsync()
        {
            var filename = GetRandomFile(@"..\..\..\Pepe", new string[] { ".png", ".jpg" });
            var builder = new EmbedBuilder()
            {
                ImageUrl = $"attachment://{filename}"
            };
            await Context.Channel.SendFileAsync(filename, embed: builder.Build());
        }

        public async Task PepeEmptyMsgAsync(SocketCommandContext context)
        {
            var filename = GetRandomFile(@"..\..\..\Pepe", new string[] { ".png", ".jpg" });
            var builder = new EmbedBuilder()
            {
                ImageUrl = $"attachment://{filename}"
            };
            await context.Channel.SendFileAsync(filename, embed: builder.Build());
        }

        private string GetRandomFile(string path, string[] extensions)
        {
            string filePath = null;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var dir = new DirectoryInfo(path);
                    var rgFiles = dir.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random rand = new Random();
                    filePath = rgFiles.ElementAt(rand.Next(0, rgFiles.Count())).FullName;
                }
                catch(Exception ex)
                {
                    throw new Exception($"File not found {ex}");
                }
            }
            return filePath;
        }
    }
}
