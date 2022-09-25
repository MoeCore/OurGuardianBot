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
        if (!player.Queue.TryDequeue(out var queueable))
        {
            await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
            return;
        }

        if (queueable is not LavaTrack track)
        {
            await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
            return;
        }

        await args.Player.PlayAsync(track);
        await args.Player.TextChannel.SendMessageAsync($"{args.Reason}: {args.Track.Title}\nNow playing: {track.Title}");
    }
}