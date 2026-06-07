using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse() : base(StatusCode.NotFound)
        {
            Body = "404 Not Found";
        }
    }
}
