using httpServer;
using httpServer.HTTP;
using httpServer.Responses;
using System.Text;
using System.Web;

public class Startup 
{
    public static async Task Main()
    {
        await DownloadSitesAsTextFile(Startup.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

        var server = new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server!"))
        .MapGet("/Redirect", new RedirectResponse("/HTML"))
        .MapGet("/HTML", new HtmlResponse(Startup.HtmlForm))
        .MapPost("/HTML", new TextResponse("Form submitted!", Startup.AddFormDataAction))
        .MapGet("/Content", new HtmlResponse(Startup.DownloadForm))
        .MapPost("/Content", new TextFileResponse(Startup.FileName))
        .MapGet("/Cookies", new HtmlResponse("", Startup.AddCookieAction))
        .MapGet("/Session", new TextResponse("", Startup.DisplaySessionInfoAction)));

        await server.Start();
    }

    // Simple in-memory session store for demo purposes
    private static readonly Dictionary<string, Session> Sessions = new();

    private static void SessionAction(Request request, Response response)
    {
        // Try get session id from request cookies
        string sessionId = null;
        if (request.Cookies.Contains(Session.SessionCookieName))
        {
            sessionId = request.Cookies[Session.SessionCookieName];
        }

        Session session = null;
        if (sessionId != null && Sessions.ContainsKey(sessionId))
        {
            session = Sessions[sessionId];
        }

        if (session == null)
        {
            // Create new session
            sessionId = Guid.NewGuid().ToString();
            session = new Session(sessionId);
            session[Session.SessionCurrentDateKey] = DateTime.UtcNow.ToString("o");
            Sessions[sessionId] = session;

            // Set session cookie via response header and also set it on the client immediately via JS
            response.Cookies.Add(Session.SessionCookieName, sessionId);

            // Client-side script writes the cookie and reloads so the next request includes the Cookie header
            var js = new StringBuilder();
            js.AppendLine("<!doctype html><html><head><meta charset='utf-8' /><title>Session</title></head><body>");
            js.AppendLine("<script>");
            js.AppendLine($"document.cookie = '{Session.SessionCookieName}={sessionId}; path=/; SameSite=Lax';");
            js.AppendLine("location.reload();");
            js.AppendLine("</script>");
            js.AppendLine("</body></html>");

            response.Body = js.ToString();
            return;
        }

        // Existing session - show stored data
        var sb = new StringBuilder();
        sb.AppendLine("<h1>Existing session</h1>");
        sb.AppendLine("<table border='1'><tr><th>Key</th><th>Value</th></tr>");
        foreach (var key in new[] { Session.SessionCurrentDateKey })
        {
            if (session.ContainsKey(key))
            {
                sb.AppendLine($"<tr><td>{HttpUtility.HtmlEncode(key)}</td><td>{HttpUtility.HtmlEncode(session[key])}</td></tr>");
            }
        }
        sb.AppendLine("</table>");
        response.Body = sb.ToString();
    }

    private static void DisplaySessionInfoAction(Request request, Response response)
    {
        var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);
        var bodyText = "";

        if (sessionExists)
        {
            var currentDate = request.Session[Session.SessionCurrentDateKey];
            bodyText = $"Session ID: {request.Session.Id}{Environment.NewLine}Session Current Date: {currentDate}";
        }
        else
        {
            response.Body = "Current date stored!.";
        }

        response.Body = "";
        response.Body += bodyText;
    }


private static async Task<string> DownloadWebSiteContent(string url)
    {

        var httpClient = new HttpClient();

        using (httpClient)
        {

            var response = await httpClient.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();
            return html.Substring(0, 2000);
        }
    }

    private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    {
        var downloads = new List<Task<string>>();

        foreach (var url in urls) 
        { 
            downloads.Add(DownloadWebSiteContent(url));
        }
        var responses = await Task.WhenAll(downloads);

        var responsesString = string.Join(Environment.NewLine + new string('-', 100), responses);

        await File.WriteAllTextAsync(fileName, responsesString);
    }
    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach (var (key, value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }

    private static void AddCookieAction(Request request, Response response)
    {
        var requestHasCookies = request.Cookies.Any(c => c.Name != Session.SessionCookieName);

        if (!requestHasCookies)
        {
            // Set cookies on the response so the browser stores them
            response.Cookies.Add("My-Cookie", "My-Value");
            response.Cookies.Add("My-Second-Cookie", "My-Second-Value");

            // Send a lightweight HTML page that immediately redirects the browser back to /Cookies
            // The subsequent request will include the Cookie header
            response.Body = "<!doctype html><html><head>" +
                "<meta http-equiv='refresh' content='0;url=/Cookies' />" +
                "</head><body></body></html>";
            return;
        }

        // When cookies are present, render them so you can inspect the values
        var cookieText = new StringBuilder();
        cookieText.AppendLine("<h1>Cookies received from the client:</h1>");
        cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");
        foreach (var cookie in request.Cookies)
        {
            cookieText.Append("<tr>");
            cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
            cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
            cookieText.Append("</tr>");
        }
        cookieText.Append("</table>");
        response.Body = cookieText.ToString();
    }
    private const string HtmlForm = 
        @"<!DOCTYPE html>
        <html>
          <head>
            <meta charset='utf-8' />
            <title>Sample Form</title>
          </head>
          <body>
            <form action='/HTML' method='POST'>
              <label>Name: <input type='text' name='Name' /></label>
              <label>Age: <input type='number' name='Age' /></label>
              <br />
              <input type='submit' value='Save' />
            </form>
          </body>
        </html>";

    private const string DownloadForm =
        @"<!DOCTYPE html>
        <html>
          <head>
            <meta charset='utf-8' />
            <title>Download Form</title>
          </head>
          <body>
            <form action='/Content' method='POST'>
              <input type='submit' value='Download Site Content' />
            </form>
          </body>
        </html>";

    private const string FileName = "content.txt";
}

