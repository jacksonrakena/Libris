using Libris.Utilities;

namespace Libris.Packets.Clientbound
{
    public class ServerListPingPongPacket : ClientboundPacket
    {
        public ServerListPingPongPacket(long confirmation)
        {
            Id = 0x01;
            Data = Converters.GetInt64Bytes(confirmation);
        }
    }
}
