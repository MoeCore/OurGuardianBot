using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class PlayModule : AudioModuleBase
{
    protected PlayModule(ILogger<PlayModule> logger, LavaNode lavaNode) : base(logger, lavaNode) { }

    [SlashCommand("play", "play audio from youtube")]
    public async Task PlayAsync(string queryOrLink)
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (!HasPlayer)
        {
            await RespondAsync(":exclamation: Bot disconnected from voice");
            return;
        }

        if (PlayerVoiceChannel != voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}");
            return;
        }

        // TODO: Play audio

        await RespondAsync("OK");
    }
}