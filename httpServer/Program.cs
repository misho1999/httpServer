using httpServer;
using httpServer.Responses;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Startup 
{
    public static void Main()
        => new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server!"))
        .MapGet("/HTML", new HtmlResponse("<h1>HTML Response</h1>"))
        .MapGet("/Redirect", new RedirectResponse("/HTML")))
        .Start();


}
