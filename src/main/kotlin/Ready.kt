import net.dv8tion.jda.api.events.GenericEvent
import net.dv8tion.jda.api.events.session.ReadyEvent
import net.dv8tion.jda.api.hooks.EventListener
import net.dv8tion.jda.api.entities.Guild
import net.dv8tion.jda.api.interactions.commands.build.Commands
import net.dv8tion.jda.api.interactions.commands.OptionType
import net.dv8tion.jda.api.interactions.commands.DefaultMemberPermissions
import net.dv8tion.jda.api.Permission

class Ready : EventListener {
    override fun onEvent(event: GenericEvent) {
        if (event !is ReadyEvent) return

        println("${event.jda.selfUser.name} ready!")

        println("found ${event.jda.guilds.count()} servers")
        event.jda.guilds.forEach { guild: Guild? ->
            if (guild == null) return@forEach

            println("${guild.name} with default channel ${guild.defaultChannel?.name}")

            guild.updateCommands().addCommands(
                Commands.slash("ping", "pong"),
                Commands.slash("bulkdelete", "remove last messages")
                    .addOption(OptionType.INTEGER, "count", "amount of messages you want to remove", true)
                    .setDefaultPermissions(DefaultMemberPermissions.enabledFor(Permission.MESSAGE_MANAGE)),
                Commands.slash("rename", "rename everyone in current server")
                    .addOption(OptionType.STRING, "name", "new name for all users or leave it empty to restore", false)
                    .setDefaultPermissions(DefaultMemberPermissions.enabledFor(Permission.NICKNAME_MANAGE)),
                Commands.slash("play", "play lofi radio in voice channel"),
                Commands.slash("stop", "quit from voice channel"),
            ).queue()
        }
    }
}
