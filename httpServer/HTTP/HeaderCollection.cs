using httpServer.Common;
using System.Collections;

namespace httpServer.HTTP
{
    public class HeaderCollection :IEnumerable<Header>
    {
        private readonly Dictionary<string, Header> headers = new Dictionary<string, Header>();

        public string this[string name] => this.headers[name].Value;

        public int Count => headers.Count;

        public bool Contains(string name)
            => headers.ContainsKey(name);

        public void Add(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));
            this.headers[name] = new Header(name, value);
        }

        public IEnumerator<Header> GetEnumerator()
        {
            return headers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
