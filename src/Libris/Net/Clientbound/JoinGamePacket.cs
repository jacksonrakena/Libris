using Libris.Models;
using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class JoinGamePacket : ClientboundPacket
    {
        public JoinGamePacket(int playerEntityId, PlayerGamemode gamemode,
            Dimension dimension, WorldType worldType, int viewDistance, bool verboseDebugInfo = true)
        {
            Id = OutboundPackets.JoinGamePacketId;

            Data = ArrayUtilities.Combine(
                Converters.GetIntBytes(playerEntityId)
                .Add((byte) gamemode),
                Converters.GetIntBytes((int) dimension)
                .Add(0),
                Converters.GetStringBytes(worldType.ToString()),
                Converters.GetVarIntBytes(viewDistance)
                .Add(Converters.GetBoolBytes(!verboseDebugInfo))
            );
        }
    }
}
