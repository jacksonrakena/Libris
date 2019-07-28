using Libris.Utilities;

namespace Libris.Net.Clientbound
{
    internal class ServerListPingPongPacket : IClientboundPacket
    {
        public ServerListPingPongPacket(long confirmation)
        {
            Id = OutboundPackets.ServerListPingPongPacketId;
            Data = Converters.GetInt64Bytes(confirmation);
        }
    }
}
