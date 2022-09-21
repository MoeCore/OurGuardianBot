using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Modules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;

    public PingModule(ILogger<PingModule> logger)
    {
        _logger = logger;
    }

    [SlashCommand("ping", "Recive a pong")]
    public async Task PingAsync()
    {
        _logger.LogTrace("Ping recived from {Username}#{UserId}", Context.User.Username, Context.User.Discriminator);
        await RespondAsync($"Pong {Context.Client.Latency}ms");
    }
}