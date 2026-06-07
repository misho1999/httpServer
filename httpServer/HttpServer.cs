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

        public async Task Start() {
            serverListener.Start();

            Console.WriteLine($"HTTP server is running on {ipAddress}:{port}");
            Console.WriteLine("Listening for connections...");

            while (true)
            {
                var connection = await serverListener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
             {

                 var networkStream = connection.GetStream();

                 string requestText = await this.ReadRequest(networkStream);

                 var request = Request.Parse(requestText);

                 Response response = (Response)this.routingTable.MatchRequest(request);

                 if (response.PreRenderAction != null)
                 {
                     response.PreRenderAction(request, response);
                 }

                 await this.WriteResponse(networkStream, (Response)response);

                 connection.Close();
             });
            }

        }

        private async Task WriteResponse(NetworkStream networkStream, Response response)
        {
            // Use the response's ToString implementation to render headers and body
            var responseText = response.ToString();
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            networkStream.Flush();
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            byte[] buffer = new byte[1024];
            StringBuilder request = new StringBuilder();
            int totalBytes = 0;
            do
            {
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
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
