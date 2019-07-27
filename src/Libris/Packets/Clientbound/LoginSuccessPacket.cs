using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    internal class LoginSuccessPacket: ClientboundPacket
    {
        public LoginSuccessPacket(string uuid, string username)
        {
            Id = OutboundPackets.LoginSuccessPacketId;
            var uuidBytes = Converters.GetStringBytes(uuid);
            var usernameBytes = Converters.GetStringBytes(username);
            Data = ArrayUtilities.Combine(uuidBytes, usernameBytes);
        }
    }
}
