using Libris.Utilities;

namespace Libris.Packets.Clientbound
{
    public class ServerListPingPongPacket : ClientboundPacket
    {
        public ServerListPingPongPacket(long confirmation)
        {
            Id = OutboundPackets.ServerListPingPongPacketId;
            Data = Converters.GetInt64Bytes(confirmation);
        }
    }
}
