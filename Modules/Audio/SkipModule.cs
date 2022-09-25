using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class SkipModule : AudioModuleBase
{
    protected SkipModule(LavaNode lavaNode) : base(lavaNode) { }

    [SlashCommand("skip", "skip to next audio")]
    public async Task SkipAsync()
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
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}.");
            return;
        }

        try
        {
            var player = LavaNode.GetPlayer(Context.Guild);

            if (player.Queue.Count == 0)
            {
                await player.StopAsync();
                await RespondAsync(":mute: Queue completed! Skipping to silence.");
                return;
            }

            if (player.Queue.TryDequeue(out var queueable) == false)
            {
                await RespondAsync(":exclamation: Something gone wrong.");
                return;
            }

            if (queueable is not LavaTrack track)
            {
                await RespondAsync("Next item in queue is not a track.");
                return;
            }

            await player.PlayAsync(track);
            await RespondAsync($":notes: Playing {track.Title} (by {track.Author}) in {voiceState.VoiceChannel.Name}.");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Can't skip audio.");
            Log.Error("[{Source}] {Message}", nameof(SkipModule), exception.Message);
        }
    }
}