import net.dv8tion.jda.api.events.interaction.command.SlashCommandInteractionEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter
import net.dv8tion.jda.api.managers.AudioManager


class Music : ListenerAdapter() {
    override fun onSlashCommandInteraction(event: SlashCommandInteractionEvent) {
        val voiceChannel = event.member?.voiceState?.channel
        val selfMember = event.guild?.selfMember
        val audioManager: AudioManager? = event.guild?.audioManager

        if (event.name == "play") {
            if (voiceChannel == null) {
                event.reply("You need to connect to voice channel").setEphemeral(true).queue()
                return
            }

            if (selfMember?.voiceState != null) {
                event.reply("Bot already in voice channel").setEphemeral(true).queue()
                return
            }

            audioManager?.sendingHandler = SendingHandler()
        }

        if (event.name == "stop") {
            if (voiceChannel == null) {
                event.reply("You need to connect to voice channel").setEphemeral(true).queue()
                return
            }

            if (selfMember?.voiceState == null) {
                event.reply("Bot already quit from voice channel").setEphemeral(true).queue()
                return
            }

            if (voiceChannel != selfMember.voiceState) {
                event.reply("You need to connect to same voice channel to do that operation").setEphemeral(true).queue()
                return
            }
        }
    }
}