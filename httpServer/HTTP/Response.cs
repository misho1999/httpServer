using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace httpServer.HTTP
{
    public class Response
    {
        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get; } = new HeaderCollection();

        // Initialize to an empty string so the non-nullable property is always set
        public string Body { get; init; } = string.Empty;

        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
            Headers.Add("Server", "MyHTTPServer/1.0");
            Headers.Add("Date", DateTime.UtcNow.ToString("r"));
        }
    }
}
