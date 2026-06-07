using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string text) : base(text, ContentType.Html)
        {

        }
    }
}
