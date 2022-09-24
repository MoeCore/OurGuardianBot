using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class PauseModule : AudioModuleBase
{
    protected PauseModule(LavaNode lavaNode) : base(lavaNode) { }

    [SlashCommand("pause", "Pause current music")]
    public async Task PauseAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (HasPlayer == false)
        {
            await RespondAsync(":exclamation: Bot disconnected from voice.");
            return;
        }

        if (PlayerVoiceChannel != voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}");
            return;
        }

        if (LavaNode.GetPlayer(Context.Guild).Track.Duration <= TimeSpan.Zero)
        {
            await RespondAsync($":exclamation: Can't pause silence");
            return;
        }

        try
        {
            await LavaNode.GetPlayer(Context.Guild).PauseAsync();
            await RespondAsync($":pause_button: Paused");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Can't play audio");
            Log.Error("[{Source}] {Message}", nameof(PauseModule), exception.Message);
        }
    }
}