using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace OurGuardian.Services;

public class DiscordSocketClientHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public DiscordSocketClientHandler(
        DiscordSocketClient client,
        IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        _client.Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, _configuration["Tokens:Discord"]);
        await _client.StartAsync();
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
}