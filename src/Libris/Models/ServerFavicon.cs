using System;
using System.IO;

namespace Libris.Models
{
    public class ServerFavicon
    {
        public string GetMinecraftFaviconString()
        {
            return _base64String;
        }

        private readonly string _base64String;

        private ServerFavicon(string base64String)
        {
            if (!base64String.StartsWith("data:image/png;base64,")) base64String = "data:image/png;base64," + base64String;
            _base64String = base64String;
        }

        public static ServerFavicon FromBase64String(string base64String)
        {
            return new ServerFavicon(base64String);
        }

        public static ServerFavicon FromFile(string fileUrl)
        {
            var bytes = File.ReadAllBytes(fileUrl);
            return new ServerFavicon(Convert.ToBase64String(bytes));
        }
    }
}
