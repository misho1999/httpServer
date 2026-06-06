using System;
using System.Collections.Generic;
using System.Text;

namespace httpServer.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public string Body { get; private set; }

        public static Request Parse(string request)
        {
            var lines = request.Split("\r\n");
            var firstLine = lines
                .First()
                .Split(" ");

            var url = firstLine[1];
            Method method = ParseMethod(firstLine[0]);
            HeaderCollection headers = ParseHeaders(lines.Skip(1));
            var bodyLines = lines.Skip(headers.Count + 2);
            string body = string.Join("\r\n", bodyLines);
            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Body = body
            };
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> lines)
        {
            var headers = new HeaderCollection();
            
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    break;

                var parts = line.Split(":");
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException($"Invalid header line: {line}");
                }
                headers.Add(parts[0].Trim(), parts[1].Trim());
            }

            return headers;
        }

        private static Method ParseMethod(string method)
        {
            try
            {
                return Enum.Parse<Method>(method);
            }
            catch (Exception)
            {

                throw new InvalidOperationException($"Method {method} is not supported.");
            }
        }
    }
}
