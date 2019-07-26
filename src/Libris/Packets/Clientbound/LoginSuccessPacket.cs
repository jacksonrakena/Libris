using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class LoginSuccessPacket: ClientboundPacket
    {
        public LoginSuccessPacket(string uuid, string username)
        {
            Id = 0x02;
            var uuidBytes = Converters.WriteUtf8String(uuid);
            var usernameBytes = Converters.WriteUtf8String(username);
            Data = ArrayUtilities.Combine(uuidBytes, usernameBytes);
        }
    }
}
