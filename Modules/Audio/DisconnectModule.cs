using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class DisconnectModule : AudioModuleBase
{
    protected DisconnectModule(LavaNode lavaNode) : base(lavaNode) { }

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
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}.");
            return;
        }

        try
        {
            await LavaNode.LeaveAsync(voiceState.VoiceChannel);
            await RespondAsync($":ok_hand: Disconnected from {voiceState.VoiceChannel.Name}.");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't disconnect from voice channel :confused:");
            Log.Error("[{Source}] {Message}", nameof(DisconnectModule), exception.Message);
        }
    }
}