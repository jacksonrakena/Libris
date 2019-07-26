using Libris.EventArgs;
using Libris.Models;
using Libris.Packets.Clientbound;
using Libris.Packets.Serverbound;
using Libris.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public Task SendPacketAsync(ClientboundPacket packet, NetworkStream streamTarget)
        {
            var packed = packet.Pack();
            Console.WriteLine($"[Outbound] Sending {packet.GetType().Name} with ID 0x{packet.Id:x2} ({packet.Data.Length} bytes)");
            return streamTarget.WriteAsync(packed, 0, packed.Length);
        }

        public ServerboundPacket ReadPacket(byte[] data)
        {
            var packet = new ServerboundPacket(data);
            Console.WriteLine($"[Inbound] Received packet with ID 0x{packet.Id:x2}");
            return packet;
        }

        public async Task StartListeningAsync()
        {
            while (true)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();
                byte[] buffer = new byte[LibrisMinecraftServer.NewConnectionBufferSize];
                var stream = client.GetStream();
                await stream.ReadAsync(buffer, 0, buffer.Length);

                var packet = ReadPacket(buffer);

                PacketReceived?.Invoke(new PacketReceivedEventArgs(client, packet));

                switch (packet.Id)
                {
                    case 0x00: // Should be the only opening packet received on all clients except 1.6 for Legacy Server Ping
                        var protocolVersion = Converters.ReadVariableInteger(packet.Data, out byte[] serverAddr);
                        var serverAddress = Converters.ReadUtf8String(serverAddr, out byte[] serverP);
                        var serverPort = Converters.ReadUnsignedShort(serverP, out byte[] nextStateR);
                        var curStateR = Converters.ReadVariableInteger(nextStateR, out byte[] contained);
                        var state = curStateR == 1 ? HandshakeRequestType.Status : HandshakeRequestType.Login;

                        var requestPacket = ReadPacket(contained);

                        Console.WriteLine($"[Handshaking] New client connecting with protocol version {protocolVersion}, using server address {serverAddress}:{serverPort}, and is requesting action {state}, with a contained packet ID of 0x{requestPacket.Id:x2}.");

                        switch (state)
                        {
                            case HandshakeRequestType.Status:
                                Console.WriteLine("[Status] Received status request.");
                                var serverListPingResponsePacket = new ServerListPingResponsePacket(LibrisMinecraftServer.ServerVersion, 
                                    LibrisMinecraftServer.ProtocolVersion, 0, _minecraftServer.MaximumPlayers, new List<PlayerListSampleEntry> {
                                        new PlayerListSampleEntry("best_jessica", "abdc8af6-70ab-4930-ab47-c6fc4e618155")
                                    }, 
                                    _minecraftServer.Description, _minecraftServer.Favicon.GetMinecraftFaviconString());
                                await SendPacketAsync(serverListPingResponsePacket, stream).ConfigureAwait(false);

                                var latencyBuffer = new byte[18];
                                await stream.ReadAsync(latencyBuffer, 0, latencyBuffer.Length);
                                var latencyPacket = ReadPacket(latencyBuffer);
                                var payload = Converters.ReadLong(latencyPacket.Data);

                                await SendPacketAsync(new ServerListPingLatencyPacket(payload), stream).ConfigureAwait(false);

                                Console.WriteLine($"[Status] Closing socket.");
                                client.Close();
                                break;
                            case HandshakeRequestType.Login:
                                var username = Converters.ReadUtf8String(requestPacket.Data, out var _);
                                Console.WriteLine("[Login] Login request initiated from user " + username);

                                // <Do authorization logic here>

                                // Login succeeded
                                await SendPacketAsync(new LoginSuccessPacket("abdc8af6-70ab-4930-ab47-c6fc4e618155", "best_jessica"), stream).ConfigureAwait(false);

                                // Instruct client to join game
                                var joinGamePacket = new JoinGamePacket(0, PlayerGamemode.Survival, Dimension.Overworld, WorldType.Default, 10);
                                await SendPacketAsync(joinGamePacket, stream).ConfigureAwait(false);

                                // Receive settings from client
                                byte[] clientSettingsBuffer = new byte[80]; // Client settings object is, at maximum, 80 bytes
                                await stream.ReadAsync(clientSettingsBuffer, 0, clientSettingsBuffer.Length);
                                var clientSettings = ReadPacket(clientSettingsBuffer);
                                var locale = Converters.ReadUtf8String(clientSettings.Data, out byte[] view_distance);

                                // AT SOME POINT, CHUNK SOME DATA HERE

                                // Inform client of global spawn
                                var spawnPositionPacket = new SpawnPositionPacket(25, 50, 2);
                                await SendPacketAsync(spawnPositionPacket, stream).ConfigureAwait(false);

                                // Send client the data of player
                                double playerX = 1.0;
                                double playerY = 1.0;
                                double playerZ = 1.0;
                                float yaw = 1.0f;
                                float pitch = 1.0f;
                                byte flags = 0x00;
                                int teleportId = 5;

                                var ppalPacket = new PlayerPositionAndLookPacket(playerX, playerY, playerZ, yaw, pitch, flags, teleportId);
                                await SendPacketAsync(ppalPacket, stream);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine($"Received unknown packet with ID 0x{packet.Id:x2}. Skipping.");
                        break;
                }
            }
        }
    }
}
