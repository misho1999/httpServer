using httpServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.HTTP
{
    public class HeaderCollection
    {
        private readonly Dictionary<string, Header> headers = new Dictionary<string, Header>();

        public int Count => headers.Count;

        public void Add(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));
            headers.Add(name, new Header(name, value));
        }
    }
}
