using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OurGuardian.Bot.Handlers;
using Serilog;

internal class Program
{
    private static async Task Main()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("Config.yml")
            .Build();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .AddSingleton(x => new CommandService(new CommandServiceConfig
                {
                    LogLevel = Discord.LogSeverity.Debug,
                    DefaultRunMode = Discord.Commands.RunMode.Async
                })))
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                .Build();

        await RunAsync(host);
    }

    private static async Task RunAsync(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        var commands = provider.GetRequiredService<InteractionService>();
        var client = provider.GetRequiredService<DiscordSocketClient>();
        var config = provider.GetRequiredService<IConfigurationRoot>();

        await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

        client.Ready += async () =>
        {
            if (IsDebug())
                await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["TestGuild"]), true);
            else
                await commands.RegisterCommandsGloballyAsync(true);
        };

        await client.LoginAsync(Discord.TokenType.Bot, config["Tokens:Discord"]);
        await client.StartAsync();

        await Task.Delay(-1);
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}