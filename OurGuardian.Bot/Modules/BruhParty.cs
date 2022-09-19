using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Bot.Modules;

public class BruhParty : InteractionModuleBase<SocketInteractionContext>
{
    public readonly ILogger _logger;

    public BruhParty(ILogger<BruhParty> logger)
    {
        _logger = logger;
    }

    [SlashCommand("Bruh", "Change all nicknames to Bruh")]
    [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
    public async Task Bruh()
    {
        _logger.LogTrace("User {Username}#{UserTag} set bruh party!", Context.User.Username, Context.User.Discriminator);

        foreach (var user in Context.Guild.Users)
        {
            await user.ModifyAsync((mod) =>
            {
                mod.Nickname = "Bruh";
            });
        }
    }
}