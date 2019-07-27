using Libris.Models;
using Libris.Packets.Clientbound;
using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Libris.Net
{
    public class LibrisTcpConnection : IDisposable
    {
        private readonly TcpClient _sender;
        private readonly NetworkStream _stream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly LibrisTcpServer _tcp;
        private LibrisMinecraftServer Server => _tcp.libris;
        private readonly int _connectionId;

        public LibrisTcpConnection(TcpClient sender, LibrisTcpServer server, int connectionId)
        {
            _stream = sender.GetStream();
            _sender = sender;
            _writer = new BinaryWriter(_stream, Encoding.UTF8, true);
            _reader = new BinaryReader(_stream, Encoding.UTF8, true);
            _tcp = server;
            _connectionId = connectionId;
        }

        public void Dispose()
        {
            _sender.Close();
            _sender.Dispose();
            _writer.Dispose();
            _reader.Dispose();
            _stream.Dispose();
            _tcp.RemoveConnection(_connectionId);
        }

        public async Task HandleAsync()
        {
            _reader.ReadVariableInteger();
            var packetId = _reader.ReadByte();

            if (packetId != 0x00)
            {
                if (packetId == 0xFE) Console.WriteLine("Received 0xFE Legacy Ping packet, the server must have sent the client incorrect data. Skipping.");
                else Console.WriteLine($"Received unknown packet with ID 0x{packetId:x2}. Skipping.");
                Dispose();
                return;
            }
            var protocolVersion = _reader.ReadVariableInteger();
            var serverAddress = _reader.ReadString();
            var serverPort = _reader.ReadUInt16BigEndian();
            var isRequestingStatus = _reader.ReadVariableInteger() == 1;

            Console.WriteLine($"[Handshaking] New client connecting with protocol version {protocolVersion}, " +
                $"using server address {serverAddress}:{serverPort}, " +
                $"and {(isRequestingStatus ? "is requesting status information" : "is requesting to login")}.");

            _reader.ReadVariableInteger();
            _reader.ReadByte();

            if (isRequestingStatus)
            {
                Console.WriteLine("[Status] Received status request.");
                var serverListPingResponsePacket = new ServerListPingResponsePacket(LibrisMinecraftServer.ServerVersion,
                    LibrisMinecraftServer.ProtocolVersion, 0, Server.MaximumPlayers, new List<PlayerListSampleEntry> {
                                        new PlayerListSampleEntry("best_jessica", "abdc8af6-70ab-4930-ab47-c6fc4e618155")
                    },
                    Server.Description, Server.Favicon?.GetMinecraftFaviconString());
                _writer.WritePacket(serverListPingResponsePacket);

                try
                {
                    var latencyPacketLength = _reader.ReadVariableInteger();
                    var latencyPacketId = _reader.ReadByte();

                    if (latencyPacketId != InboundPackets.ServerListLatencyPingPacketId)
                    {
                        Console.WriteLine($"[Status] Closing socket. Client did not request latency detection.");
                        Dispose();
                        return;
                    }

                    var payload = _reader.ReadInt64();

                    _writer.WritePacket(new ServerListPingPongPacket(payload));

                    Console.WriteLine($"[Status] Closing socket.");
                    Dispose();
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine($"[Status] Closing socket. Client did not request latency detection - received End of Stream. Perhaps the response data was corrupt?");
                    Dispose();
                }
            }
            else
            {
                var username = _reader.ReadString();
                Console.WriteLine("[Login] Login request initiated from user " + username);

                // <Do authorization logic here>

                // Login succeeded
                _writer.WritePacket(new LoginSuccessPacket("abdc8af6-70ab-4930-ab47-c6fc4e618155", "best_jessica"));

                // Instruct client to join game
                var joinGamePacket = new JoinGamePacket(0, PlayerGamemode.Survival, Dimension.Overworld, WorldType.Default, 10);
                _writer.WritePacket(joinGamePacket);

                // Receive settings from client
                _reader.ReadVariableInteger();
                var clientSettingsId = _reader.ReadByte();
                if (clientSettingsId != InboundPackets.ClientSettingsPacketId)
                {
                    Console.WriteLine($"[Login] Expected byte {InboundPackets.ClientSettingsPacketId} Client Settings, received 0x{clientSettingsId:x2} instead. Closing socket.");
                    Dispose();
                    return;
                }
                var locale = _reader.ReadString();
                var viewDistance = _reader.ReadSByte();
                var chatMode = _reader.ReadVariableInteger();
                var chatColors = _reader.ReadBoolean();
                var displayedSkinPartBitMask = _reader.ReadByte();
                var mainHand = _reader.ReadVariableInteger();
                Console.WriteLine("[Login] User " + username + " registered client settings with locale " + locale + ", view distance " + Convert.ToInt32(viewDistance) + ", and main hand " + mainHand);

                // AT SOME POINT, CHUNK SOME DATA HERE

                // Inform client of global spawn
                var spawnPositionPacket = new SpawnPositionPacket(25, 50, 2);
                _writer.WritePacket(spawnPositionPacket);

                // Send client the data of player
                double playerX = 1.0;
                double playerY = 1.0;
                double playerZ = 1.0;
                float yaw = 1.0f;
                float pitch = 1.0f;
                byte flags = 0x00;
                int teleportId = 5;

                var ppalPacket = new PlayerPositionAndLookPacket(playerX, playerY, playerZ, yaw, pitch, flags, teleportId);
                _writer.WritePacket(ppalPacket);
            }
        }
    }
}
