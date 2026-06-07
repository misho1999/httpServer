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
                string request = ReadRequest(networkStream);
                Console.WriteLine( request);
                WriteResponse(networkStream, "Hello, World!");
                connection.Close();
            }

        }

        private void WriteResponse(NetworkStream networkStream, string content)
        {
            byte[] body = Encoding.UTF8.GetBytes(content);
            int contentLength = body.Length;

            string headers =
                $"HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain; charset=utf-8\r\n" +
                $"Content-Length: {contentLength}\r\n" +
                "Connection: close\r\n" +
                "\r\n"; // blank line separates headers from body

            byte[] headerBytes = Encoding.UTF8.GetBytes(headers);
            networkStream.Write(headerBytes, 0, headerBytes.Length);
            networkStream.Write(body, 0, body.Length);
            networkStream.Flush();
        }

        private string ReadRequest(NetworkStream networkStream)
        {
            byte[] buffer = new byte[1024];
            StringBuilder request = new StringBuilder();
            int totalBytes = 0;
            do
            {
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                totalBytes += bytesRead;

                if(totalBytes > 10 * 1024) // Limit request size to 10KB
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                request.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            } while (networkStream.DataAvailable);
            return request.ToString();
        }
}
}
