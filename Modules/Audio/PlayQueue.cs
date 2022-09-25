using Victoria;
using Victoria.EventArgs;

namespace OurGuardian.Modules.Audio;

public class PlayQueue : AudioModuleBase
{
    protected PlayQueue(LavaNode lavaNode) : base(lavaNode)
    {
        lavaNode.OnTrackEnded += OnTrackEnded;
    }

    private async Task OnTrackEnded(TrackEndedEventArgs args)
    {
        if (args.Reason != Victoria.Enums.TrackEndReason.Finished)
            return;

        var player = args.Player;

        if (player.Queue.Count == 0)
            return;

        if (player.Queue.TryDequeue(out var queueable) == false)
        {
            await player.TextChannel.SendMessageAsync(":exclamation: Something gone wrong.");
            return;
        }

        if (queueable is not LavaTrack track)
        {
            await player.TextChannel.SendMessageAsync(":exclamation: Next item in queue is not a track.");
            return;
        }

        await player.PlayAsync(track);
        await player.TextChannel.SendMessageAsync($":notes: Playing {track.Title} (by {track.Author}) in {player.VoiceChannel.Name}.");
    }
}