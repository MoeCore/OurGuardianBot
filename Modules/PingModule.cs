using Discord.Interactions;

namespace OurGuardian.Modules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Recive a pong")]
    public async Task PingAsync()
    {
        await RespondAsync($"Pong {Context.Client.Latency}ms");
    }
}