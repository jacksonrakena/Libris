using Libris.Models;
using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class JoinGamePacket : IClientboundPacket
    {
        private readonly int _playerEntityId;
        private readonly byte _playerGamemode;
        private readonly int _dimension;
        private readonly int _maxPlayersIgnored = 0;
        private readonly string _worldType;
        private readonly int _viewDistance;
        private readonly bool _hideVerboseDebugInfo;

        public JoinGamePacket(int playerEntityId, PlayerGamemode gamemode,
            Dimension dimension, WorldType worldType, int viewDistance, bool verboseDebugInfo = true)
        {
            /*Data = ArrayUtilities.Combine(
                Converters.GetIntBytes(playerEntityId)
                .Add((byte) gamemode),
                Converters.GetIntBytes((int) dimension)
                .Add(0),
                Converters.GetStringBytes(worldType.ToString()),
                Converters.GetVarIntBytes(viewDistance)
                .Add(Converters.GetBoolBytes(!verboseDebugInfo))
            );*/

            _playerEntityId = playerEntityId;
            _playerGamemode = (byte) gamemode;
            _dimension = (int) dimension;
            _worldType = worldType.ToString();
            _viewDistance = viewDistance;
            _hideVerboseDebugInfo = !verboseDebugInfo;
        }

        public void WriteToStream(NetworkStream stream)
        {

        }
    }
}
