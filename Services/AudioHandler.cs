using Discord.WebSocket;
using Victoria;

namespace OurGuardian.Services;

public class AudioHandler
{
    private readonly DiscordSocketClient _client;
    private readonly LavaNode _lavaNode;

    public AudioHandler(DiscordSocketClient client, LavaNode lavaNode)
    {
        _client = client;
        _lavaNode = lavaNode;
    }

    public Task InitializeAsync()
    {
        _client.Ready += ClientReady;
        return Task.CompletedTask;
    }

    private async Task ClientReady()
    {
        if (_lavaNode.IsConnected == false)
            await _lavaNode.ConnectAsync();
    }
}