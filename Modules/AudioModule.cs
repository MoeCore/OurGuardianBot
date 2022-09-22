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

    [SlashCommand("connect", "connect to voice channel")]
    public async Task ConnectAsync()
    {
        _logger.LogTrace("{Username}#{UserId} used connect command", Context.User.Username, Context.User.Discriminator);

        if (_lavaNode.HasPlayer(Context.Guild))
        {
            await RespondAsync("Already **connected** to voice");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await RespondAsync("You must be **connected** to a voice channel!");
            return;
        }

        try
        {
            await RespondAsync($"Connected to **{voiceState.VoiceChannel.Name}**!");
            await _lavaNode.JoinAsync(voiceState.VoiceChannel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(ConnectAsync)}");
        }
    }

    [SlashCommand("disconnect", "disconnect from voice channel")]
    public async Task DisconnectAsync()
    {
        _logger.LogTrace("{Username}#{UserId} used disconnect command", Context.User.Username, Context.User.Discriminator);

        if (_lavaNode.HasPlayer(Context.Guild) == false)
        {
            await RespondAsync("Already **disconnected** from voice");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await RespondAsync("You must be **connected** to a voice channel!");
            return;
        }

        try
        {
            await RespondAsync($"Disconnected from **{voiceState.VoiceChannel.Name}**!");
            await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(DisconnectAsync)}");
        }
    }

    [SlashCommand("play", "play music from youtube link or query")]
    public async Task PlayAsync(string queryOrLink)
    {
        if (_lavaNode.HasPlayer(Context.Guild) == false)
        {
            await RespondAsync("Connect bot to voice!");
            return;
        }

        int playlistInUrlIndex = queryOrLink.IndexOf("&list=");
        if (playlistInUrlIndex >= 0)
            queryOrLink = queryOrLink[..playlistInUrlIndex];

        var search = await _lavaNode.SearchYouTubeAsync(queryOrLink);
        switch (search.Status)
        {
            case Victoria.Responses.Search.SearchStatus.TrackLoaded:
                break;
            case Victoria.Responses.Search.SearchStatus.PlaylistLoaded:
                break;
            case Victoria.Responses.Search.SearchStatus.SearchResult:
                break;
            case Victoria.Responses.Search.SearchStatus.NoMatches:
                await RespondAsync("Nothing found");
                break;
            case Victoria.Responses.Search.SearchStatus.LoadFailed:
                await RespondAsync("Load failed. The url could be wrong or maybe LavaLink needs an update.");
                break;
        }

        var voiceState = (Context.User as IVoiceState)!;
        var audio = search.Tracks.First();

        try
        {
            await RespondAsync($"Playing **{audio.Title}** by **{audio.Author}** in **{voiceState.VoiceChannel.Name}**!");
            await _lavaNode.GetPlayer(Context.Guild).PlayAsync(audio);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(PlayAsync)}");
        }
    }
}