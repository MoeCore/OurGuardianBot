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
            await ReplyAsync("I'm **already connected** to a voice channel!");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await ReplyAsync("You must be **connected** to a voice channel!");
            return;
        }

        try
        {
            await ReplyAsync($"Connected to **{voiceState.VoiceChannel.Name}**!");
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
            await ReplyAsync("I'm **already disconnected** from voice channel!");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await ReplyAsync("You must be **connected** to a voice channel!");
            return;
        }

        try
        {
            await ReplyAsync($"Disconnected from **{voiceState.VoiceChannel.Name}**!");
            await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(DisconnectAsync)}");
        }
    }

    [SlashCommand("play", "disconnect from voice channel")]
    public async Task PlayAsync(string queryOrLink)
    {
        await ConnectAsync();

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
                await ReplyAsync("Nothing found");
                break;
            case Victoria.Responses.Search.SearchStatus.LoadFailed:
                await ReplyAsync("Load failed. The url could be wrong or maybe LavaLink needs an update.");
                break;
        }

        var voiceState = (Context.User as IVoiceState)!;
        var audio = search.Tracks.First();

        try
        {
            await ReplyAsync($"Playing {audio.Title} by {audio.Author} in **{voiceState.VoiceChannel.Name}**!");
            await _lavaNode.GetPlayer(Context.Guild).PlayAsync(audio);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"[{nameof(AudioModule)}] Catch exception on {nameof(PlayAsync)}");
        }
    }
}