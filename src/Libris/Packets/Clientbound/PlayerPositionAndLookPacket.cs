using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class PlayerPositionAndLookPacket : ClientboundPacket
    {
        public PlayerPositionAndLookPacket(double playerX, double playerY, double playerZ, float yaw, float pitch, byte flags, int teleportConfirmationId)
        {
            Id = 0x32;
            Data = ArrayUtilities.Combine(
                Converters.WriteDouble(playerX),
                Converters.WriteDouble(playerY),
                Converters.WriteDouble(playerZ),
                Converters.WriteFloat(yaw),
                Converters.WriteFloat(pitch)
                .Add(flags),
                Converters.WriteVariableInteger(teleportConfirmationId)
            );
        }
    }
}
