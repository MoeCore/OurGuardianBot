using Discord;
using Discord.Interactions;
using OurGuardian.Modules.Audio.Interactions;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class PlayModule : AudioModuleBase
{
    protected PlayModule(LavaNode lavaNode) : base(lavaNode) { }

    [SlashCommand("play", "play audio from youtube")]
    public async Task PlayAsync(string queryOrLink)
    {
        var search = await LavaNode.SearchYouTubeAsync(queryOrLink);

        if (search.Status == Victoria.Responses.Search.SearchStatus.LoadFailed)
        {
            await RespondAsync("Load failed. The url could be wrong or maybe LavaLink needs an update.");
            return;
        }

        if (search.Status == Victoria.Responses.Search.SearchStatus.NoMatches)
        {
            await RespondAsync("Nothing found.");
            return;
        }

        var searchResult = search.Tracks.Take(5).ToArray();
        var componentBuilder = new ComponentBuilder();
        var embedBuilder = new EmbedBuilder()
            .WithColor(new Color(255, 0, 0))
            .WithTitle($"Youtube search results: {queryOrLink}")
            .WithThumbnailUrl(await searchResult.First().FetchArtworkAsync());

        for (int i = 0; i < searchResult.Length; i++)
        {
            var currentItem = searchResult[i];
            var displayTitle = $"{i + 1}. {currentItem.Author}";

            embedBuilder.AddField(displayTitle, currentItem.Title);
            componentBuilder.WithButton(displayTitle, nameof(PlayModuleInteraction.PlayComponentResponseAsync) + currentItem.Url);
        }

        await RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
    }
}