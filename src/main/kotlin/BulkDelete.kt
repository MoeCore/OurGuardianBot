import net.dv8tion.jda.api.events.interaction.command.SlashCommandInteractionEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter

class BulkDelete : ListenerAdapter() {
    override fun onSlashCommandInteraction(event: SlashCommandInteractionEvent) {
        if (event.name != "bulkdelete") return

        val count = event.getOption("count")?.asInt ?: 0

        event.channel.iterableHistory.takeAsync(count).thenAccept(event.channel::purgeMessages)
        event.reply("Done!").setEphemeral(true).queue()
    }
}