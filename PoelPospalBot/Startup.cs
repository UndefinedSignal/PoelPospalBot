using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Logging;
using Lavalink4NET.MemoryCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoelPospalBot.Services;

namespace PoelPospalBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .Add<WritableJsonConfigurationSource>((Action<WritableJsonConfigurationSource>)(s =>
                {
                    s.FileProvider = null;
                    s.Path = "Config/_config.json";
                    s.Optional = false;
                    s.ReloadOnChange = true;
                    s.ResolveFileProvider();
                }));
            Configuration = builder.Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var Startup = new Startup(args);
            await Startup.RunAsync();
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<CommandsHandler>();
            provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<TemporaryLoopService>();
            provider.GetRequiredService<WelcomeService>();
            provider.GetRequiredService<Lavalink4NET.IAudioService>();
            provider.GetRequiredService<ILogger>();

            await provider.GetRequiredService<StartupService>().StartAsync();
            await provider.GetRequiredService<Lavalink4NET.IAudioService>().InitializeAsync();
            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 2000,
                GatewayIntents = GatewayIntents.All
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = Discord.Commands.RunMode.Async,
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandsHandler>()
            .AddSingleton<StartupService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<TemporaryLoopService>()
            .AddSingleton<WelcomeService>()
            .AddSingleton<Lavalink4NET.IAudioService, Lavalink4NET.LavalinkNode>()
            .AddSingleton<Lavalink4NET.IDiscordClientWrapper, Lavalink4NET.DiscordNet.DiscordClientWrapper>()
            .AddSingleton<ILogger, EventLogger>()
            .AddSingleton(new Lavalink4NET.LavalinkNodeOptions
            {
                Password = "123",
                WebSocketUri = "ws://localhost:2333/",
                RestUri = "http://localhost:2333/",

            })
            .AddSingleton<Lavalink4NET.ILavalinkCache, LavalinkCache>()

            .AddSingleton(Configuration);

        }
    }
}
