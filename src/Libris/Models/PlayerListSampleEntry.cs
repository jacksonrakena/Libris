using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Libris.Models
{
    public class PlayerListSampleEntry
    {
        [JsonProperty("name")]
        public string Username { get; }

        [JsonProperty("id")]
        public string UUID { get; }

        public PlayerListSampleEntry(string username, string uuid)
        {
            Username = username;
            UUID = uuid;
        }
    }
}
