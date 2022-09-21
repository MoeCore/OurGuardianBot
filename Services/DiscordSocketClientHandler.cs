using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Services;

public class DiscordSocketClientHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public DiscordSocketClientHandler(
        DiscordSocketClient client,
        IConfiguration configuration,
        ILogger<DiscordSocketClientHandler> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _client.Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, _configuration["Tokens:Discord"]);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("DiscordSocketClient: {message}", logMessage.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError("DiscordSocketClient: {message}", logMessage.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("DiscordSocketClient: {message}", logMessage.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("DiscordSocketClient: {message}", logMessage.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace("DiscordSocketClient: {message}", logMessage.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("DiscordSocketClient: {message}", logMessage.Message);
                break;
        }

        return Task.CompletedTask;
    }
}