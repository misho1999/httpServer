using httpServer.Common;

namespace httpServer.HTTP
{
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Header(string _name, string _value)
        {
            Guard.AgainstNull(_name, nameof(_name));
            Guard.AgainstNull(_value, nameof(_value));
            Name = _name;
            Value = _value;
        }
    }
}
