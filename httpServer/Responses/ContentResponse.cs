using httpServer.Common;
using httpServer.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.Responses
{
    public class ContentResponse : Response
    {
        public ContentResponse(string content, string contentType, Action<Request, Response> preRenderAction = null) : base(StatusCode.OK)
        {
            Guard.AgainstNull(content);
            Guard.AgainstNull(contentType);

            this.Body = content;
            this.PreRenderAction = preRenderAction;

            this.Headers.Add(Header.ContentType, contentType);
        }

        public override string ToString()
        {
            if (this.Body != null)
            {
                var contentLength = Encoding.UTF8.GetByteCount(this.Body).ToString();
                this.Headers.Add(Header.ContentLength, contentLength);
            }

            return base.ToString();
        }
    }
}