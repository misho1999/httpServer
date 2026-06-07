using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class UnauthorizedResponse : Response
    {
        public UnauthorizedResponse() : base(StatusCode.Unauthorized)
        {
            Body = "401 Unauthorized";
        }
    }
}
