using httpServer;
using httpServer.HTTP;
using httpServer.Responses;

public class Startup 
{
    public static void Main()
        => new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server!"))
        .MapGet("/Redirect", new RedirectResponse("/HTML"))
        .MapGet("/HTML", new HtmlResponse(Startup.HtmlForm))
        .MapPost("/HTML", new TextResponse("Form submitted!", Startup.AddFormDataAction)))
        .Start();

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
}

