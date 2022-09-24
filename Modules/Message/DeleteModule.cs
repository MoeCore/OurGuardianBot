using Discord;
using Discord.Interactions;

namespace OurGuardian.Modules.Message;

public class DeleteModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("bulkdelete", "Remove messages")]
    [DefaultMemberPermissions(GuildPermission.ManageMessages)]
    public async Task BulkDeleteAsync(int amount)
    {
        await RespondAsync($"Removing {amount} messages");
        IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
    }
}