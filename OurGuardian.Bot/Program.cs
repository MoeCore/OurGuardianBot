using System.Reflection;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        client.Ready += async () =>
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
#if DEBUG
            await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["Guilds:Test"]), true);
#else
            await commands.RegisterCommandsGloballyAsync(true);
#endif
        };

        client.InteractionCreated += async interaction =>
        {
            var scope = provider.CreateScope();
            var context = new SocketInteractionContext(client, interaction);
            await commands.ExecuteCommandAsync(context, scope.ServiceProvider);
        };

        await client.LoginAsync(Discord.TokenType.Bot, config["Tokens:Discord"]);
        await client.StartAsync();

        await Task.Delay(-1);
    }
}