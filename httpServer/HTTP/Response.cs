using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
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

        // Initialize to an empty string so the non-nullable property is always set
        public string Body { get; init; } = string.Empty;
        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");
            foreach(var header in this.Headers)
            {
                result.AppendLine(header.ToString());
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
