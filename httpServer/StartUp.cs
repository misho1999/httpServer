using httpServer;
using httpServer.HTTP;
using httpServer.Responses;

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
        .MapPost("/Content", new TextFileResponse(Startup.FileName)));

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

