using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string text, Action<Request, Response> preRenderAction = null) : base(text, ContentType.Plain, preRenderAction)
        {
        }
    }
}
