using Discord;
using Discord.Interactions;
using Victoria;

namespace OurGuardian.Modules.Audio;

public class AudioModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly LavaNode LavaNode;

    protected AudioModuleBase(LavaNode lavaNode) => LavaNode = lavaNode;
    protected IVoiceChannel PlayerVoiceChannel => LavaNode.GetPlayer(Context.Guild).VoiceChannel;
    protected bool HasPlayer => LavaNode.HasPlayer(Context.Guild);

    protected async Task<IVoiceState?> GetVoiceStateOrSayConnectAsync()
    {
        var voiceState = Context.User as IVoiceState;
        if (voiceState == null || voiceState.VoiceChannel == null)
            await RespondAsync(":exclamation: You must be connected to a voice channel.");

        return voiceState;
    }
}