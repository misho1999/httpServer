using httpServer.HTTP;
using httpServer.Routing;
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

        private readonly RoutingTable routingTable;

        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.serverListener = new TcpListener(this.ipAddress, this.port);
           
            routingTableConfiguration(this.routingTable = new RoutingTable());   
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable) : this("127.0.0.1", port, routingTable)
        {

        }

        public HttpServer(Action<IRoutingTable> routingTable) : this(55000, routingTable)
        {
        }

        public void Start() {
            serverListener.Start();

            Console.WriteLine($"HTTP server is running on {ipAddress}:{port}");
            Console.WriteLine("Listening for connections...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();

                var networkStream = connection.GetStream();

                string requestText = this.ReadRequest(networkStream);
                Console.WriteLine(requestText);

                var request = Request.Parse(requestText);

                var response = this.routingTable.MatchRequest(request);

                WriteResponse(networkStream, (Response)response);
                connection.Close();
            }

        }

        private void WriteResponse(NetworkStream networkStream, Response response)
        {
            byte[] body = Encoding.UTF8.GetBytes(response.Body ?? string.Empty);
            int contentLength = body.Length;

            var sb = new StringBuilder();
            sb.AppendLine($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}");

            bool hasContentLength = false;
            foreach (var header in response.Headers)
            {
                if (string.Equals(header.Name, Header.ContentLength, StringComparison.OrdinalIgnoreCase))
                {
                    hasContentLength = true;
                }

                sb.AppendLine(header.ToString());
            }

            if (!hasContentLength)
            {
                sb.AppendLine($"{Header.ContentLength}: {contentLength}");
            }

            sb.AppendLine(); // blank line separates headers from body

            byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
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
