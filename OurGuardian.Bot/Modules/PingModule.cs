using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Modules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;

    public PingModule(ILogger<PingModule> logger)
    {
        _logger = logger;
    }

    [SlashCommand("ping", "Recive a pong")]
    public async Task Ping()
    {
        _logger.LogInformation("Ping recived from {Username}#{UserId}", Context.User.Username, Context.User.Discriminator);
        await RespondAsync("Pong");
    }
}