using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Modules;

public class NicknameUpdater : InteractionModuleBase<SocketInteractionContext>
{
    public readonly ILogger _logger;

    public NicknameUpdater(ILogger<NicknameUpdater> logger)
    {
        _logger = logger;
    }

    [SlashCommand("setnicknames", "Change all nicknames to Bruh")]
    [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
    public async Task SetNicknames(string newName)
    {
        _logger.LogTrace("User {Username}#{UserTag} set all nicknames to {newName}!", Context.User.Username, Context.User.Discriminator, newName);

        await ReplyAsync($"Updating all nicknames to {newName}");

        await Context.Guild.DownloadUsersAsync();
        foreach (var user in Context.Guild.Users)
        {
            try
            {
                await user.ModifyAsync((mod) =>
                {
                    mod.Nickname = newName;
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"[{nameof(NicknameUpdater)}] Catch exception on {nameof(SetNicknames)}");
            }

            await Task.Delay(100);
        }
    }

    [SlashCommand("resetnicknames", "Change all nicknames to Bruh")]
    [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
    public async Task ResetNicknames()
    {
        _logger.LogTrace("User {Username}#{UserTag} reset set all nicknames to default!", Context.User.Username, Context.User.Discriminator);

        await ReplyAsync($"Resetting all nicknames");

        await Context.Guild.DownloadUsersAsync();
        foreach (var user in Context.Guild.Users)
        {
            try
            {
                await user.ModifyAsync((mod) =>
                {
                    mod.Nickname = "";
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"[{nameof(NicknameUpdater)}] Catch exception on {nameof(ResetNicknames)}");
            }

            await Task.Delay(100);
        }
    }
}