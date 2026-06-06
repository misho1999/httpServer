namespace httpServer.HTTP
{
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Header(string _name, string _value)
        {
            Name = _name;
            Value = _value;
        }
    }
}
