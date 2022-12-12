import io.github.cdimascio.dotenv.dotenv
import net.dv8tion.jda.api.JDABuilder
import net.dv8tion.jda.api.requests.GatewayIntent
import net.dv8tion.jda.api.utils.ChunkingFilter
import net.dv8tion.jda.api.utils.MemberCachePolicy

val dotenv = dotenv { directory = System.getProperty("user.dir") }

fun main() {
    val jda = JDABuilder.createDefault(dotenv["DISCORD_BOT_TOKEN"])
        .setChunkingFilter(ChunkingFilter.ALL)
        .setMemberCachePolicy(MemberCachePolicy.ALL)
        .enableIntents(GatewayIntent.MESSAGE_CONTENT)
        .enableIntents(GatewayIntent.GUILD_MEMBERS)
        .addEventListeners(Ready())
        .addEventListeners(Ping())
        .addEventListeners(BulkDelete())
        .addEventListeners(Rename())
        .addEventListeners(Music())
        .build()

    jda.awaitReady()
}