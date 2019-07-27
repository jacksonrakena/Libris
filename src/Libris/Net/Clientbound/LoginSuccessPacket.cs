using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class LoginSuccessPacket: ClientboundPacket
    {
        private readonly string _uuid;
        private readonly string _username;

        public LoginSuccessPacket(string uuid, string username)
        {
            Id = OutboundPackets.LoginSuccessPacketId;
            Data = new byte[uuid.Length + username.Length];
        }

        public void WriteToStream(NetworkStream stream)
        {
            Span<byte> data = stackalloc byte[_uuid.Length + _username.Length + 1];
            data[0] = OutboundPackets.LoginSuccessPacketId;
            Converters.GetStringBytes(_uuid, data.Slice(1));
            Converters.GetStringBytes(_username, data.Slice(_uuid.Length + 1));

            Span<byte> dataLengthBytes = stackalloc byte[5];
            Converters.GetVarIntBytes(data.Length, dataLengthBytes, out int dataLengthBytesLength);

            stream.Write(dataLengthBytes.Slice(0, dataLengthBytesLength));
            stream.Write(data);
        }
    }
}
