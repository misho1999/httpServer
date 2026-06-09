using httpServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.HTTP
{
    public class Cookie
    {
        public Cookie(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        override public string ToString() => $"{Name}={Value}";

    }
}
