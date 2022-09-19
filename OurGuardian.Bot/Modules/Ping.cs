using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Modules;

public class Ping : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;

    public Ping(ILogger<Ping> logger)
    {
        _logger = logger;
    }

    [SlashCommand("ping", "Recive a pong")]
    public async Task Pong()
    {
        _logger.LogTrace("Ping recived from {Username}#{UserId}", Context.User.Username, Context.User.Discriminator);
        await RespondAsync("Pong");
    }
}