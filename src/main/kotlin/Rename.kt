import net.dv8tion.jda.api.events.interaction.command.SlashCommandInteractionEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter

class Rename : ListenerAdapter() {
    override fun onSlashCommandInteraction(event: SlashCommandInteractionEvent) {
        if (event.name != "rename") return

        val guild = event.guild
        val members = guild?.members
        val nickname = event.getOption("name")?.asString

        println("Found ${members?.count()} in ${guild?.name}")

        if (nickname == null)
            println("Resetting nicknames in ${guild?.name}")
        else
            println("Renaming everyone in ${guild?.name} to $nickname")

        members?.forEach { member ->
            if (!guild.selfMember.canInteract(member)) return@forEach

            if (nickname == null)
                member.modifyNickname(null).queue()
            else
                member.modifyNickname(nickname).queue()
        }

        event.reply("Done!").setEphemeral(true).queue()
    }
}