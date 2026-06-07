using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class BadRequestResponse : Response
    {
        public BadRequestResponse() : base(StatusCode.BadRequest)
        {
            Body = "400 Bad Request";
        }
    }
}
