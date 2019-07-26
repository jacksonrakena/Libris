using Libris.EventArgs;
using Libris.Models;
using Libris.Packets.Clientbound;
using Libris.Packets.Serverbound;
using Libris.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Libris.Net
{
    internal class LibrisTcpServer
    {
        public delegate Task PacketReceivedHandler(PacketReceivedEventArgs packetReceived);
        public event PacketReceivedHandler PacketReceived;

        private readonly TcpListener _tcpListener;
        private readonly LibrisMinecraftServer _minecraftServer;

        public LibrisTcpServer(LibrisMinecraftServer server)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 25565);
            _tcpListener.Start();
            _minecraftServer = server;
        }

        /*public Task SendPacketAsync(ClientboundPacket packet, NetworkStream streamTarget)
        {
            var packed = packet.Pack();
            Console.WriteLine($"[Outbound] Sending {packet.GetType().Name} with ID 0x{packet.Id:x2} ({packet.Data.Length} bytes)");
            return streamTarget.WriteAsync(packed, 0, packed.Length);
        }*/

        /*public ServerboundPacket ReadPacket(byte[] data)
        {
            var packet = new ServerboundPacket(data);
            Console.WriteLine($"[Inbound] Received packet with ID 0x{packet.Id:x2}");
            return packet;
        }*/

        /*public ServerboundPacket ReadPacket(BinaryReader reader)
        {
            var length = reader.ReadVariableInteger();
            var id = reader.ReadByte();
            var data = reader.ReadBytes(length - 1);
            return new ServerboundPacket(id, data);
        }*/

        public async Task StartListeningAsync()
        {
            while (true)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                using var reader = new BinaryReader(stream, Encoding.UTF8, true);
                using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

                var packetLength = reader.ReadVariableInteger();
                var packetId = reader.ReadByte();

                //PacketReceived?.Invoke(new PacketReceivedEventArgs(client, packet));

                switch (packetId)
                {
                    case 0x00: // Should be the only opening packet received on all clients except 1.6 for Legacy Server Ping
                        var protocolVersion = reader.ReadVariableInteger();
                        var serverAddress = reader.ReadString();
                        var serverPort = reader.ReadUInt16BigEndian();
                        var curStateR = reader.ReadVariableInteger();
                        var state = curStateR == 1 ? HandshakeRequestType.Status : HandshakeRequestType.Login;

                        Console.WriteLine($"[Handshaking] New client connecting with protocol version {protocolVersion}, using server address {serverAddress}:{serverPort}, and is requesting action {state}.");

                        switch (state)
                        {
                            case HandshakeRequestType.Status:
                                Console.WriteLine("[Status] Received status request.");
                                var serverListPingResponsePacket = new ServerListPingResponsePacket(LibrisMinecraftServer.ServerVersion, 
                                    LibrisMinecraftServer.ProtocolVersion, 0, _minecraftServer.MaximumPlayers, new List<PlayerListSampleEntry> {
                                        new PlayerListSampleEntry("best_jessica", "abdc8af6-70ab-4930-ab47-c6fc4e618155")
                                    }, 
                                    _minecraftServer.Description, _minecraftServer.Favicon.GetMinecraftFaviconString());
                                await writer.WritePacketAsync(serverListPingResponsePacket).ConfigureAwait(false);

                                var latencyPacketLength = reader.ReadVariableInteger();
                                var latencyPacketId = reader.ReadByte();
                                var payload = reader.ReadInt64();

                                await writer.WritePacketAsync(new ServerListPingLatencyPacket(payload)).ConfigureAwait(false);

                                Console.WriteLine($"[Status] Closing socket.");
                                client.Close();
                                break;
                            case HandshakeRequestType.Login:
                                reader.ReadVariableInteger();
                                reader.ReadByte();
                                var username = reader.ReadString();
                                Console.WriteLine("[Login] Login request initiated from user " + username);

                                // <Do authorization logic here>

                                // Login succeeded
                                await writer.WritePacketAsync(new LoginSuccessPacket("abdc8af6-70ab-4930-ab47-c6fc4e618155", "best_jessica")).ConfigureAwait(false);

                                // Instruct client to join game
                                var joinGamePacket = new JoinGamePacket(0, PlayerGamemode.Survival, Dimension.Overworld, WorldType.Default, 10);
                                await writer.WritePacketAsync(joinGamePacket).ConfigureAwait(false);

                                // Receive settings from client
                                reader.ReadVariableInteger();
                                reader.ReadByte();
                                var locale = reader.ReadString();
                                var viewDistance = reader.ReadSByte();
                                var chatMode = reader.ReadVariableInteger();
                                var chatColors = reader.ReadBoolean();
                                var displayedSkinPartBitMask = reader.ReadByte();
                                var mainHand = reader.ReadVariableInteger();
                                Console.WriteLine("[Login] User " + username + " registered client settings with locale " + locale);

                                // AT SOME POINT, CHUNK SOME DATA HERE

                                // Inform client of global spawn
                                var spawnPositionPacket = new SpawnPositionPacket(25, 50, 2);
                                await writer.WritePacketAsync(spawnPositionPacket).ConfigureAwait(false);

                                // Send client the data of player
                                double playerX = 1.0;
                                double playerY = 1.0;
                                double playerZ = 1.0;
                                float yaw = 1.0f;
                                float pitch = 1.0f;
                                byte flags = 0x00;
                                int teleportId = 5;

                                var ppalPacket = new PlayerPositionAndLookPacket(playerX, playerY, playerZ, yaw, pitch, flags, teleportId);
                                await writer.WritePacketAsync(ppalPacket);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine($"Received unknown packet with ID 0x{packetId:x2}. Skipping.");
                        break;
                }
            }
        }
    }
}
