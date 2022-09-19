using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Services;

public class Log
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger _logger;
    private readonly InteractionService _interactionService;

    public Log(DiscordSocketClient client, ILogger<Log> logger, InteractionService interactionService)
    {
        _client = client;
        _logger = logger;
        _interactionService = interactionService;
    }

    public Task Init()
    {
        _client.Log += logMessage =>
        {
            _logger.LogInformation("Discord socket client: {message}", logMessage.Message);
            return Task.CompletedTask;
        };

        _interactionService.Log += logMessage =>
        {
            _logger.LogInformation("Interaction service: {message}", logMessage.Message);
            return Task.CompletedTask;
        };

        return Task.CompletedTask;
    }
}