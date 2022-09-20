using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Services;

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
        _logger.LogInformation("DiscordSocketClient: {message}", logMessage.Message);
        return Task.CompletedTask;
    }
}