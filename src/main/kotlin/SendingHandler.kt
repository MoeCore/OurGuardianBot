import net.dv8tion.jda.api.audio.AudioSendHandler
import java.nio.ByteBuffer

class SendingHandler : AudioSendHandler {
    override fun canProvide(): Boolean {
        TODO("Not yet implemented")
    }

    override fun provide20MsAudio(): ByteBuffer? {
        TODO("Not yet implemented")
    }

}
