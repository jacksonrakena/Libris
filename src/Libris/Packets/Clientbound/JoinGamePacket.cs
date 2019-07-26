using Libris.Models;
using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class JoinGamePacket : ClientboundPacket
    {
        public JoinGamePacket(int playerEntityId, PlayerGamemode gamemode,
            Dimension dimension, WorldType worldType, int viewDistance, bool verboseDebugInfo = true)
        {
            Id = 0x25;

            Data = ArrayUtilities.Combine(
                Converters.WriteInteger(playerEntityId)
                .Add((byte) gamemode),
                Converters.WriteInteger((int) dimension)
                .Add(0),
                Converters.WriteUtf8String(worldType.ToString()),
                Converters.WriteVariableInteger(viewDistance)
                .Add(Converters.WriteBoolean(!verboseDebugInfo))
            );
        }
    }
}
