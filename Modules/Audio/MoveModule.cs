using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class MoveModule : AudioModuleBase
{
    protected MoveModule(ILogger<MoveModule> logger, LavaNode lavaNode) : base(logger, lavaNode) { }

    [SlashCommand("move", "Move bot to current voice channel")]
    [DefaultMemberPermissions(Discord.GuildPermission.MoveMembers)]
    public async Task MoveAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (HasPlayer == false)
        {
            await RespondAsync(":exclamation: Bot disconnected from voice.");
            return;
        }

        if (PlayerVoiceChannel == voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: Bot already in {PlayerVoiceChannel.Name}");
            return;
        }

        try
        {
            await LavaNode.MoveChannelAsync(voiceState.VoiceChannel);
            await RespondAsync($":notes: Moved to {voiceState.VoiceChannel.Name}");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't disconnect from voice channel :confused:");
            Logger.LogError(exception, $"[{nameof(MoveModule)}] Catch exception on {nameof(MoveAsync)}");
        }
    }
}