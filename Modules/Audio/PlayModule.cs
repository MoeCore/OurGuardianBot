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
            await ReplyAsync("Load failed. The url could be wrong or maybe LavaLink needs an update.");
            return;
        }

        if (search.Status == Victoria.Responses.Search.SearchStatus.NoMatches)
        {
            await ReplyAsync("Nothing found");
            return;
        }

        var searchResult = search.Tracks.Take(5).ToArray();
        var componentBuilder = new ComponentBuilder();
        var embedBuilder = new EmbedBuilder()
            .WithColor(new Color(255, 0, 0))
            .WithTitle("Youtube search results:")
            .WithThumbnailUrl(await searchResult.First().FetchArtworkAsync());

        for (int i = 0; i < searchResult.Length; i++)
        {
            string displayNumber = (i + 1).ToString();
            var currentItem = searchResult[i];

            embedBuilder.AddField(currentItem.Author, currentItem.Title);
            componentBuilder.WithButton(displayNumber, nameof(PlayModuleInteraction.PlayComponentResponseAsync) + currentItem.Url);
        }

        await RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
    }
}