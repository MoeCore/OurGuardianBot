using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class DisconnectModule : AudioModuleBase
{
    protected DisconnectModule(ILogger<DisconnectModule> logger, LavaNode lavaNode) : base(logger, lavaNode) { }

    [SlashCommand("disconnect", "disconnect from voice channel")]
    public async Task DisconnectAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (HasPlayer == false)
        {
            await RespondAsync(":exclamation: Already disconnected from voice.");
            return;
        }

        if (PlayerVoiceChannel != voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}");
            return;
        }

        try
        {
            await LavaNode.LeaveAsync(voiceState.VoiceChannel);
            await RespondAsync($":notes: Disconnected from {voiceState.VoiceChannel.Name}");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't disconnect from voice channel :confused:");
            Logger.LogError(exception, $"[{nameof(DisconnectModule)}] Catch exception on {nameof(DisconnectAsync)}");
        }
    }
}