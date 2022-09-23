using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class ConnectModule : AudioModuleBase
{
    protected ConnectModule(ILogger<ConnectModule> logger, LavaNode lavaNode) : base(logger, lavaNode) { }

    [SlashCommand("connect", "connect to voice channel")]
    public async Task ConnectAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (HasPlayer)
        {
            await RespondAsync(":exclamation: Already connected to voice.");
            return;
        }

        try
        {
            await LavaNode.JoinAsync(voiceState.VoiceChannel);
            await RespondAsync($":notes: Connected to {voiceState.VoiceChannel.Name}");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't connect to voice channel :confused:");
            Logger.LogError(exception, $"[{nameof(ConnectModule)}] Catch exception on {nameof(ConnectAsync)}");
        }
    }
}