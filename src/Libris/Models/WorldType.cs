using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Models
{
    public class WorldType
    {
        private readonly string _value;

        private WorldType(string value)
        {
            _value = value;
        }

        public static WorldType Default => new WorldType("default");
        public static WorldType Flat => new WorldType("flat");
        public static WorldType LargeBiomes => new WorldType("largeBiomes");
        public static WorldType Amplified => new WorldType("amplified");
        public static WorldType Default_1_1 => new WorldType("default_1_1");

        public override string ToString()
        {
            return _value;
        }
    }
}
