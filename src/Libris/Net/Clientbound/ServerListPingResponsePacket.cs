using Libris.Models;
using Libris.Utilities;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;

namespace Libris.Net.Clientbound
{
    internal class ServerListPingResponsePacket : ClientboundPacket
    {
        private readonly string _serializedData;

        public ServerListPingResponsePacket(string serverVersion, int protocolVersion, int currentPlayers, int maximumPlayers,
            List<PlayerListSampleEntry> onlinePlayerSample, ChatText serverDescription, string faviconString = null)
        {
            _serializedData = JsonSerializer.Serialize(new ServerListPingResponse(
                        new ServerListPingResponseVersion(serverVersion, protocolVersion),
                        new ServerListPingResponsePlayerList(maximumPlayers, currentPlayers, onlinePlayerSample),
                        serverDescription,
                        faviconString
                    ), new JsonSerializerOptions { IgnoreNullValues = true });
#if DEBUG // Is this supposed to be shown on release? :thinking:
            Console.WriteLine(_serializedData);
#endif
        }

        internal void WriteToStream(NetworkStream stream)
        {
            var serializedDataByteCount = Encoding.UTF8.GetByteCount(_serializedData);
            Span<byte> dataVarIntPrefixBytes = stackalloc byte[5];
            Converters.GetVarIntBytes(serializedDataByteCount, dataVarIntPrefixBytes, out int dataVarIntLength);

            Span<byte> lengthBytesSpan = stackalloc byte[5];
            Converters.GetVarIntBytes(dataVarIntLength + serializedDataByteCount + 1, lengthBytesSpan, out int lengthBytesLength);
            var lengthBytes = lengthBytesSpan.Slice(0, lengthBytesLength);

            Span<byte> data = stackalloc byte[dataVarIntLength + lengthBytes.Length + 1 + serializedDataByteCount];

            lengthBytes.CopyTo(data);
            data[lengthBytes.Length] = OutboundPackets.ServerListPingResponsePacketId;
            dataVarIntPrefixBytes.Slice(0, dataVarIntLength).CopyTo(data.Slice(lengthBytes.Length + 1));

            Encoding.UTF8.GetBytes(_serializedData, data.Slice(dataVarIntLength + lengthBytes.Length + 1));

            stream.Write(data);
        }

        internal class ServerListPingResponse
        {
            [JsonPropertyName("version")]
            public ServerListPingResponseVersion Version { get; }

            [JsonPropertyName("players")]
            public ServerListPingResponsePlayerList Players { get; }

            [JsonPropertyName("description")]
            public ChatText Description { get; }

            [JsonPropertyName("favicon")]
            public string FaviconString { get; }

            public ServerListPingResponse(ServerListPingResponseVersion version, ServerListPingResponsePlayerList players,
                ChatText description, string faviconString)
            {
                Version = version;
                Players = players;
                Description = description;
                FaviconString = faviconString;
            }
        }

        internal class ServerListPingResponseVersion
        {
            [JsonPropertyName("name")]
            public string ServerVersion { get; }
            [JsonPropertyName("protocol")]
            public int ProtocolVersion { get; }

            public ServerListPingResponseVersion(string serverVersion, int protocolVersion)
            {
                ServerVersion = serverVersion;
                ProtocolVersion = protocolVersion;
            }
        }

        internal class ServerListPingResponsePlayerList
        {
            [JsonPropertyName("max")]
            public int MaximumPlayers { get; }

            [JsonPropertyName("online")]
            public int OnlinePlayers { get; }

            [JsonPropertyName("sample")]
            public List<PlayerListSampleEntry> OnlinePlayerSample { get; }

            public ServerListPingResponsePlayerList(int maxPlayers, int onlinePlayers, List<PlayerListSampleEntry> sample)
            {
                MaximumPlayers = maxPlayers;
                OnlinePlayers = onlinePlayers;
                OnlinePlayerSample = sample;
            }
        }
    }
}
