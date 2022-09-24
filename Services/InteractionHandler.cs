using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace OurGuardian.Services;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly DebugChecker _debugChecker;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService handler,
        IServiceProvider services,
        IConfiguration config,
        DebugChecker debugChecker)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _debugChecker = debugChecker;
    }

    public async Task InitializeAsync()
    {
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;

        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.InteractionCreated += HandleInteraction;
    }

    private static async Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };

        Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        try
        {
            if (_debugChecker.IsDebug)
            {
                ulong testGuildId = _configuration.GetValue<ulong>("Guilds:Test");
                await _handler.RegisterCommandsToGuildAsync(testGuildId);
                Log.Information("Registering commands to {id}", testGuildId);
            }
            else
            {
                await _handler.RegisterCommandsGloballyAsync();
                Log.Information("Registering commands globally");
            }
        }
        catch (Exception exception)
        {
            Log.Information(exception, $"[{nameof(InteractionHandler)}] Catch exception on {nameof(ReadyAsync)}");
        }
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _handler.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // TODO: Implement UnmetPrecondition
                        break;
                    default:
                        break;
                }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (message) => await message.Result.DeleteAsync());
        }
    }
}