using Libris.EventArgs;
using Libris.Models;
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

        private ConnectionState _state = ConnectionState.NotConnected;

        public LibrisTcpServer(LibrisMinecraftServer server)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 25565);
            _tcpListener.Start();
            _minecraftServer = server;
        }

        public async Task StartListeningAsync()
        {
            while (true)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();
                byte[] buffer = new byte[1024];
                var stream = client.GetStream();
                stream.Read(buffer, 0, buffer.Length);

                var packetLength = Converters.ReadVariableInteger(buffer, out byte[] remainder);
                var packetId = Converters.ReadByte(remainder, out byte[] r0);

                Console.WriteLine($"Received packet 0x{packetId:x2} with length of {packetLength} bytes. Current state: {_state}");

                PacketReceived?.Invoke(new PacketReceivedEventArgs(client, packetId, r0));

                switch (packetId)
                {
                    case 0x00: // handshake
                        Console.WriteLine($"[Packet Switcher] Received 0x00 packet, current connection state is {_state}");
                        var protocolVersion = Converters.ReadVariableInteger(r0, out byte[] serverAddr);
                        var serverAddress = Converters.ReadUtf8String(serverAddr, out byte[] serverP);
                        var serverPort = Converters.ReadUnsignedShort(serverP, out byte[] nextStateR);
                        var curStateR = Converters.ReadVariableInteger(nextStateR, out byte[] empty);
                        var state = curStateR == 1 ? ConnectionState.Status : ConnectionState.Login;
                        Console.WriteLine($"[Handshaking] Protocol version: {protocolVersion} | Server address: {serverAddress} | Port: {serverPort} | State requested: {state}");

                        if (state == ConnectionState.Status)
                        {
                            var requestPacketLength = Converters.ReadVariableInteger(empty, out byte[] id);
                            var requestPacketId = Converters.ReadByte(id, out byte[] empty1);
                            Console.WriteLine("Received status request with " + requestPacketLength + $" bytes and 0x{requestPacketId:x2} as it's ID.");
                            var d1 = JsonConvert.SerializeObject(
                            new ServerListPingResponse(new ServerListPingResponseVersion(LibrisMinecraftServer.ServerVersion, LibrisMinecraftServer.ProtocolVersion),
                            new ServerListPingResponsePlayerList(_minecraftServer.MaximumPlayers, 0, new List<int> { }),
                            new ServerListPingResponseDescription(_minecraftServer.Description))
                            );
                            new ClientboundPacket(0x00, d1).WriteToStream(stream);
                            Console.WriteLine($"[Status] Wrote status data to output stream");
                            client.Close();
                        }

                        if (state == ConnectionState.Login)
                        {
                            var requestPacketLength = Converters.ReadVariableInteger(empty, out byte[] id);
                            var requestPacketId = Converters.ReadByte(id, out byte[] empty1);
                            var username = Converters.ReadUtf8String(empty1, out byte[] empty2);
                            Console.WriteLine("Received login start from user " + username + " with " + requestPacketLength + $" bytes and 0x{requestPacketId:x2} as it's ID.");

                            // send 0x02 LOGIN SUCCESS

                            var data = Converters.WriteUtf8String("abdc8af6-70ab-4930-ab47-c6fc4e618155").Concat(Converters.WriteUtf8String("best_jessica")).ToArray();
                            new ClientboundPacket(0x02, data).WriteToStream(stream);
                            Console.WriteLine($"[Login] Wrote Login Success to stream");

                            // send 0x25 JOIN GAME

                            var entityIdBytes = Converters.WriteInteger(0);
                            Console.WriteLine(string.Join(" ", entityIdBytes.Select(a => a.ToString())));
                            var gamemodeByte = (byte) 0; // survival
                            var dimensionInt = Converters.WriteInteger(0); // overworld
                            //var difficultyByte = (byte) 0; // peaceful REMOVED IN 1.14.4
                            var maxPlayers = (byte) 0;
                            var levelType = Converters.WriteUtf8String("default"); // world type
                            var viewDistance = Converters.WriteVariableInteger(10); // view distance
                            var reducedDebugInfo = Converters.WriteBoolean(false); // show debug data

                            var packet = new ClientboundPacket(0x25, entityIdBytes.Append(gamemodeByte).Concat(dimensionInt).Append(maxPlayers).Concat(levelType).Concat(viewDistance).Append(reducedDebugInfo).ToArray());
                            packet.WriteToStream(stream);
                            Console.WriteLine($"[Login] Wrote Join Game to stream");

                            byte[] clientSettingsBuffer = new byte[1024];
                            var clientSettings = await stream.ReadAsync(clientSettingsBuffer);
                            var clientSettingsLength = Converters.ReadVariableInteger(clientSettingsBuffer, out byte[] cs_packetId);
                            var clientSettingsPacketId = Converters.ReadVariableInteger(cs_packetId, out byte[] cs_data);
                            var locale = Converters.ReadUtf8String(cs_data, out byte[] view_distance);
                            Console.WriteLine("[Login] Client sent client settings with locale " + locale);

                            // AT SOME POINT, CHUNK SOME DATA HERE
                            
                            // tell client of it's spawn position
                            var locationX = 25;
                            var locationY = 50;
                            var locationZ = 2;

                            var locationInt = ((locationX & 0x3FFFFFF) << 38) | ((locationZ & 0x3FFFFFF) << 12) | (locationY & 0xFFF);
                            Console.WriteLine("Spawnpoint: " + locationInt);
                            var location = Converters.WriteUnsignedLong((ulong) locationInt);
                            var spawnPositionPacket = new ClientboundPacket(0x49, location);
                            spawnPositionPacket.WriteToStream(stream);
                            Console.WriteLine("[Login] Wrote Spawn Position to stream");

                            // player position and look
                            double playerX = 1.0;
                            double playerY = 1.0;
                            double playerZ = 1.0;
                            float yaw = 1.0f;
                            float pitch = 1.0f;
                            byte flags = 0x00;
                            int teleportId = 5;

                            var ppalData = Converters.WriteDouble(playerX)
                                .Concat(Converters.WriteDouble(playerY))
                                .Concat(Converters.WriteDouble(playerZ))
                                .Concat(Converters.WriteFloat(yaw))
                                .Concat(Converters.WriteFloat(pitch))
                                .Append(flags)
                                .Concat(Converters.WriteVariableInteger(teleportId))
                                .ToArray();

                            var ppalPacket = new ClientboundPacket(0x32, ppalData);
                            ppalPacket.WriteToStream(stream);
                            Console.WriteLine("[Login] Wrote PPAL to stream");
                        }
                        break;
                    default:
                        Console.WriteLine("Received unknown packet with ID 0x" + packetId.ToString("x2"));
                        break;
                }
            }
        }
    }
}
