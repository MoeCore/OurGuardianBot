using Discord.Interactions;
using Serilog;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class ClearModule : AudioModuleBase
{
    protected ClearModule(LavaNode lavaNode) : base(lavaNode) { }

    [SlashCommand("clear", "Clear all tracks in queue")]
    public async Task ClearAsync()
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
                await RespondAsync(":exclamation: Queue alredy empty.");
                return;
            }

            for (; player.Queue.Count != 0;)
                player.Queue.TryDequeue(out _);

            await RespondAsync(":white_check_mark: Queue cleared.");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Can't clear queue");
            Log.Error("[{Source}] {Message}", nameof(ClearModule), exception.Message);
        }
    }
}