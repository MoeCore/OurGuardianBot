using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class ResumeModule : AudioModuleBase
{
    protected ResumeModule(LavaNode lavaNode) : base(lavaNode) { }

    [SlashCommand("resume", "Resume current music")]
    public async Task ResumeAsync()
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
            await RespondAsync($":exclamation: Resuming silence");
            return;
        }

        try
        {
            await LavaNode.GetPlayer(Context.Guild).ResumeAsync();
            await RespondAsync($":arrow_forward: Resume");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Can't play audio");
            Log.Error("[{Source}] {Message}", nameof(ResumeModule), exception.Message);
        }
    }
}