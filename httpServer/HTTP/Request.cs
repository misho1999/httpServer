using System.Web;

namespace httpServer.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public string Body { get; private set; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }

        public static Request Parse(string request)
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                throw new InvalidOperationException("Request is empty.");
            }

            var lines = request.Split("\r\n");
            if (lines.Length == 0)
            {
                throw new InvalidOperationException("Request is empty.");
            }

            var firstLineRaw = lines.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstLineRaw))
            {
                throw new InvalidOperationException("Invalid request line.");
            }

            var firstLine = firstLineRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (firstLine.Length < 2)
            {
                throw new InvalidOperationException($"Invalid request line: {firstLineRaw}");
            }

            var url = firstLine[1];
            Method method = ParseMethod(firstLine[0]);
            HeaderCollection headers = ParseHeaders(lines.Skip(1));
            var cookies = ParseCookies(headers);
            var bodyLines = lines.Skip(headers.Count + 2);
            string body = string.Join("\r\n", bodyLines);
            var form = ParseForm(headers, body);
            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Cookies = cookies,
                Body = body,
                Form = form

            };
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookieCollection = new CookieCollection();
            if (headers.Contains(Header.Cookie))
            {
                var cookieHeader = headers[Header.Cookie];
                var allCookies = cookieHeader.Split(';');
                    
                foreach (var cookieText in allCookies)
                {
                    var cookieParts = cookieText.Split('=');
                   var cookieName = cookieParts[0].Trim();
                    var cookieValue = cookieParts[1].Trim();

                    cookieCollection.Add(cookieName, cookieValue);
                }
            }
            return cookieCollection;
        }

        private static IReadOnlyDictionary<string, string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();
            if (headers.Contains(Header.ContentType))
            {
                var contentTypeHeader = headers[Header.ContentType] ?? string.Empty;
                // Compare media type only, ignore optional charset or parameters
                if (contentTypeHeader.Split(';', 2)[0].Trim().Equals("application/x-www-form-urlencoded", System.StringComparison.OrdinalIgnoreCase))
                {
                    var parsedResult = ParseFormData(body);
                    foreach (var (name, value) in parsedResult)
                    {
                        formCollection.Add(name, value);
                    }
                }
            }
            return formCollection;
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> lines)
        {
            var headers = new HeaderCollection();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    break;

                int idx = line.IndexOf(':');
                if (idx <= 0)
                {
                    throw new InvalidOperationException($"Invalid header line: {line}");
                }

                var name = line.Substring(0, idx).Trim();
                var value = line.Substring(idx + 1).Trim();
                headers.Add(name, value);
            }

            return headers;
        }

        private static Method ParseMethod(string method)
        {
            if (Enum.TryParse<Method>(method, true, out var parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException($"Method {method} is not supported.");
        }

        private static Dictionary<string, string> ParseFormData(string bodyLines)
            => HttpUtility.UrlDecode(bodyLines)
            .Split('&')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(
                part => part[0],
                part => part[1],
                StringComparer.InvariantCultureIgnoreCase);


    }
}
