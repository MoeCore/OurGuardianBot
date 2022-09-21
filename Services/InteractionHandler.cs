using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Services;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService handler,
        IServiceProvider services,
        IConfiguration config,
        ILogger<InteractionHandler> logger)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _client.Ready += ReadyAsync;
        _handler.Log += LogAsync;

        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.InteractionCreated += HandleInteraction;
    }

    private Task LogAsync(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("InteractionService: {message}", logMessage.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError("InteractionService: {message}", logMessage.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("InteractionService: {message}", logMessage.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("InteractionService: {message}", logMessage.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace("InteractionService: {message}", logMessage.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("InteractionService: {message}", logMessage.Message);
                break;
        }

        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        try
        {
            if (Program.IsDebug())
            {
                ulong testGuildId = _configuration.GetValue<ulong>("Guilds:Test");
                await _handler.RegisterCommandsToGuildAsync(testGuildId, true);
                _logger.LogInformation("Registering commands to {id}", testGuildId);
            }
            else
            {
                await _handler.RegisterCommandsGloballyAsync(true);
                _logger.LogInformation("Registering commands globally");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(InteractionHandler)}] Catch exception on {nameof(ReadyAsync)}");
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