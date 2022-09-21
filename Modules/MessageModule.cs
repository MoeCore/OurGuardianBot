using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace OurGuardian.Modules;

public class MessageModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger _logger;

    public MessageModule(ILogger<MessageModule> logger)
    {
        _logger = logger;
    }

    [SlashCommand("bulkdelete", "Remove messages")]
    [DefaultMemberPermissions(GuildPermission.ManageMessages)]
    public async Task BulkDeleteAsync(int amount)
    {
        IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
    }
}