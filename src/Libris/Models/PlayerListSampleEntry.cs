using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Libris.Models
{
    public class PlayerListSampleEntry
    {
        [JsonPropertyName("name")]
        public string Username { get; }

        [JsonPropertyName("id")]
        public string UUID { get; }

        public PlayerListSampleEntry(string username, string uuid)
        {
            Username = username;
            UUID = uuid;
        }
    }
}
