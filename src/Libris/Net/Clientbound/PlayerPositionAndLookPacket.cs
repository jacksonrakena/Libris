using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class PlayerPositionAndLookPacket : ClientboundPacket
    {
        public PlayerPositionAndLookPacket(double playerX, double playerY, double playerZ, float yaw, float pitch, byte flags, int teleportConfirmationId)
        {
            Id = OutboundPackets.PlayerPositionAndLookPacketId;
            Data = ArrayUtilities.Combine(
                Converters.GetDoubleBytes(playerX),
                Converters.GetDoubleBytes(playerY),
                Converters.GetDoubleBytes(playerZ),
                Converters.GetFloatBytes(yaw),
                Converters.GetFloatBytes(pitch)
                .Add(flags),
                Converters.GetVarIntBytes(teleportConfirmationId)
            );
        }
    }
}
