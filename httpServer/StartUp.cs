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
        .MapGet("/Cookies", new HtmlResponse("", Startup.AddCookieAction)));

        await server.Start();
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
        var requestHasCookies = request.Cookies.Any();
        var bodyText = "";

        if (requestHasCookies)
        {
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

            bodyText = cookieText.ToString();
        }
        else
        {
            bodyText = "<h1>Cookies Set!</h1>";
        }

        // Ensure the generated HTML is returned to the client
        response.Body = bodyText;

        // Always set cookies on the response so the browser will store them for subsequent requests
        response.Cookies.Add("My-Cookie", "My-Value");
        response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
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

