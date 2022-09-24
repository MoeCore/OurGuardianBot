using Discord;
using Discord.Interactions;
using Serilog;

namespace OurGuardian.Modules;

public class NicknameModule : InteractionModuleBase<SocketInteractionContext>
{
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
                Log.Error("[{Source}] {Message}", nameof(NicknameModule), exception.Message);
            }
        }
    }
}