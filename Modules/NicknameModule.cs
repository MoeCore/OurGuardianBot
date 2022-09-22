using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Modules;

public class NicknameModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;

    public NicknameModule(ILogger<NicknameModule> logger) => _logger = logger;

    [SlashCommand("setnicknames", "Change all nicknames")]
    [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
    public async Task SetNicknamesAsync(string newName)
    {
        await RespondAsync($"Setting all nicknames to {newName} (may take some time)");
        await ApplyNickNamesAsync(newName);
    }

    [SlashCommand("resetnicknames", "Reset all nicknames")]
    [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
    public async Task ResetNicknamesAsync()
    {
        await RespondAsync($"Resetting all nicknames (may take some time)");
        await ApplyNickNamesAsync();
    }

    private async Task ApplyNickNamesAsync(string? newName = null)
    {
        await Context.Guild.DownloadUsersAsync();
        foreach (var user in Context.Guild.Users)
        {
            try
            {
                await user.ModifyAsync((mod) =>
                {
                    mod.Nickname = newName ?? "";
                });
                await Task.Delay(200);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"[{nameof(NicknameModule)}] Catch exception on {nameof(ApplyNickNamesAsync)}");
            }
        }
    }
}