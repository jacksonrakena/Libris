using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class LoginSuccessPacket: IClientboundPacket
    {
        private readonly string _uuid;
        private readonly string _username;

        public LoginSuccessPacket(string uuid, string username)
        {
            _uuid = uuid;
            _username = username;
        }

        public void WriteToStream(NetworkStream stream)
        {
            var usernameByteCount = Constants.Encoding.GetByteCount(_username);
            var uuidByteCount = Constants.Encoding.GetByteCount(_uuid);

            Span<byte> uuidLengthBytes = stackalloc byte[5];
            Converters.GetVarIntBytes(uuidByteCount, uuidLengthBytes, out int uuidLengthBytesLength);

            Span<byte> usernameLengthBytes = stackalloc byte[5];
            Converters.GetVarIntBytes(usernameByteCount, usernameLengthBytes, out int usernameLengthBytesLength);

            var dataLength = 1 + uuidLengthBytesLength + usernameLengthBytesLength + Constants.Encoding.GetByteCount(_uuid) + Constants.Encoding.GetByteCount(_username);

            Span<byte> packetLengthBytes = stackalloc byte[5];
            Converters.GetVarIntBytes(uuidLengthBytesLength + usernameLengthBytesLength + 1, packetLengthBytes, out int packetLengthBytesLength); ;

            Span<byte> data = stackalloc byte[dataLength + packetLengthBytesLength];
            packetLengthBytes.Slice(0, packetLengthBytesLength).CopyTo(data);
            data[packetLengthBytesLength + 1] = OutboundPackets.LoginSuccessPacketId;
            uuidLengthBytes.Slice(0, uuidLengthBytesLength).CopyTo(data.Slice(packetLengthBytesLength + 1));
            Constants.Encoding.GetBytes(_uuid, data.Slice(packetLengthBytesLength + 1 + uuidLengthBytesLength));
            usernameLengthBytes.Slice(0, usernameLengthBytesLength).CopyTo(data.Slice(packetLengthBytesLength + 1 + uuidLengthBytesLength + uuidByteCount));
            Constants.Encoding.GetBytes(_username, data.Slice(packetLengthBytesLength + 1 + uuidLengthBytesLength + uuidByteCount + usernameLengthBytesLength));
            stream.Write(data);
        }
    }
}
