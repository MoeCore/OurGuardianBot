using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio.Interactions;

public class PlayModuleInteraction : AudioModuleBase
{
    protected PlayModuleInteraction(LavaNode lavaNode) : base(lavaNode) { }

    [ComponentInteraction(nameof(PlayComponentResponseAsync) + "*")]
    public async Task PlayComponentResponseAsync(string trackUrl)
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (!HasPlayer)
        {
            await RespondAsync(":exclamation: Bot disconnected from voice.");
            return;
        }

        if (PlayerVoiceChannel != voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: You need to connect to {PlayerVoiceChannel.Name}.");
            return;
        }

        // TODO: Remove original interaction message
        var track = (await LavaNode.SearchYouTubeAsync(trackUrl)).Tracks.First();

        try
        {
            var player = LavaNode.GetPlayer(Context.Guild);
            if (player.PlayerState == Victoria.Enums.PlayerState.Playing || player.PlayerState == Victoria.Enums.PlayerState.Paused)
            {
                LavaNode.GetPlayer(Context.Guild).Queue.Enqueue(track);
                await RespondAsync($":notes: Adding {track.Title} (by {track.Author}) to queue.");
            }
            else
            {
                await LavaNode.GetPlayer(Context.Guild).PlayAsync(track);
                await RespondAsync($":notes: Playing {track.Title} (by {track.Author}) in {voiceState.VoiceChannel.Name}.");
            }

        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Can't play audio");
            Log.Error("[{Source}] {Message}", nameof(PlayModule), exception.Message);
        }
    }
}