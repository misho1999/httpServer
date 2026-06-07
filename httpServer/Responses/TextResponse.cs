using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string text) : base(text, ContentType.Plain)
        {
        }
    }
}
