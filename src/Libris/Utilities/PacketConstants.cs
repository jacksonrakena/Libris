using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Utilities
{
    public static class InboundPackets
    {
        public const byte ClientSettingsPacketId = 0x05;
        public const byte ServerListLatencyPingPacketId = 0x01;
    }
    
    public static class OutboundPackets
    {
        public const byte JoinGamePacketId = 0x25;
        public const byte LoginSuccessPacketId = 0x02;
        public const byte PlayerPositionAndLookPacketId = 0x32;
        public const byte ServerListPingPongPacketId = 0x01;
        public const byte ServerListPingResponsePacketId = 0x00;
        public const byte SpawnPositionPacketId = 0x49;
    }
}
