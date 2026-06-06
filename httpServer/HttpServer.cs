using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace httpServer
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        public HttpServer(string _ipAddress, int _port)
        {
            ipAddress = IPAddress.Parse(_ipAddress);
            port = _port;
            serverListener = new TcpListener(ipAddress, port);
        }

        public void Start() {
            serverListener.Start();

            Console.WriteLine($"HTTP server is running on {ipAddress}:{port}");
            Console.WriteLine("Listening for connections...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();
                var networkStream = connection.GetStream();
                WriteResponse(networkStream, "Hello, World!");
                connection.Close();
            }

        }

        private static void WriteResponse(NetworkStream networkStream, string content)
        {
            int contentLength = Encoding.UTF8.GetByteCount(content);
            string response = $@"HTTP/1.1 200 OK
                              Content-Type: text/plain; charset=utf-8
                              Content-Length: {contentLength}

{content}";
            var responseBytes = Encoding.UTF8.GetBytes(response);
            networkStream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
