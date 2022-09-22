using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Victoria;

namespace OurGuardian.Modules;

public class AudioModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;
    private readonly LavaNode _lavaNode;

    public AudioModule(ILogger<AudioModule> logger, LavaNode lavaNode)
    {
        _logger = logger;
        _lavaNode = lavaNode;
    }

    public IVoiceChannel GetPlayerVoiceChannel => _lavaNode.GetPlayer(Context.Guild).VoiceChannel;

    [SlashCommand("connect", "connect to voice channel")]
    public async Task ConnectAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (_lavaNode.HasPlayer(Context.Guild))
        {
            await RespondAsync(":exclamation: Already connected to voice.");
            return;
        }

        try
        {
            await _lavaNode.JoinAsync(voiceState.VoiceChannel);
            await RespondAsync($":notes: Connected to {voiceState.VoiceChannel.Name}");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't connect to voice channel :confused:");
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(ConnectAsync)}");
        }
    }

    [SlashCommand("disconnect", "disconnect from voice channel")]
    public async Task DisconnectAsync()
    {
        var voiceState = await GetVoiceStateOrSayConnectAsync();
        if (voiceState == null || voiceState.VoiceChannel == null) return;

        if (_lavaNode.HasPlayer(Context.Guild) == false)
        {
            await RespondAsync(":exclamation: Already disconnected from voice.");
            return;
        }

        if (GetPlayerVoiceChannel != voiceState.VoiceChannel)
        {
            await RespondAsync($":exclamation: You need to connect to {GetPlayerVoiceChannel.Name}");
            return;
        }

        try
        {
            await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
            await RespondAsync($":notes: Disconnected from {voiceState.VoiceChannel.Name}");
        }
        catch (Exception exception)
        {
            await RespondAsync($":exclamation: Oops can't disconnect from voice channel :confused:");
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(ConnectAsync)}");
        }
    }

    public async Task<IVoiceState?> GetVoiceStateOrSayConnectAsync()
    {
        var voiceState = Context.User as IVoiceState;
        if (voiceState == null || voiceState.VoiceChannel == null)
            await RespondAsync(":exclamation: You must be connected to a voice channel.");

        return voiceState;
    }
}