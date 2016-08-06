namespace Syringe.Core.Http
{
    public class HttpHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public HttpHeader() : this("", "")
        {
        }

        public HttpHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}