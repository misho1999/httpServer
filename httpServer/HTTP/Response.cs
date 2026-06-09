using System.Text;

namespace httpServer.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers.Add(Header.Server, "MyHTTPServer/1.0");
            this.Headers.Add(Header.Date, DateTime.UtcNow.ToString("r"));
        }
        public StatusCode StatusCode { get; init; }


        public HeaderCollection Headers { get; } = new HeaderCollection();

        public CookieCollection Cookies { get; } = new CookieCollection();

        // Initialize to an empty string so the non-nullable property is always set
        public string Body { get; set; } = string.Empty;

        public Action<Request, Response> PreRenderAction { get; protected set; }
        public override string ToString()
        {
            var result = new StringBuilder();
            // Ensure Content-Length header is present and correct (measured in UTF-8 bytes)
            var bodyBytes = Encoding.UTF8.GetBytes(this.Body ?? string.Empty);
            if (!this.Headers.Contains(Header.ContentLength))
            {
                this.Headers.Add(Header.ContentLength, bodyBytes.Length.ToString());
            }

            result.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

            foreach(var header in this.Headers)
            {
                result.AppendLine(header.ToString());
            }

            foreach (var cookie in this.Cookies)
            {
                result.AppendLine($"{Header.SetCookie}: {cookie}");
            }

            result.AppendLine(); // blank line separates headers from body

            if (!string.IsNullOrEmpty(this.Body))

            {
                result.AppendLine(this.Body);
            }

            return result.ToString();
        }

      
    }
}
