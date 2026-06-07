using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class RedirectResponse : Response
    {
        public RedirectResponse(string location) : base(StatusCode.Found)
        {
            Headers.Add(Header.Location, location);
        }
    }
}
