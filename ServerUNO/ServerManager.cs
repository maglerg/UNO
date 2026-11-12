using ServerUNO;
using Shared_Library;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Server
{
    public class ServerManager
    {
        private HttpListener listener;
        private RoomManager roomManager;

        public ServerManager(RoomManager roomManager)
        {
            this.roomManager = roomManager;
            listener = new HttpListener();
        }

        public async Task Start()
        {
            listener.Prefixes.Add("http://localhost:5000/ws/");
            listener.Start();

            Console.WriteLine("Server started...");

            while (true)
            {
                HttpListenerContext context =
                    await listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    HandleClient(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async void HandleClient(HttpListenerContext context)
        {
            HttpListenerWebSocketContext wsContext =
                await context.AcceptWebSocketAsync(null);

            WebSocket socket = wsContext.WebSocket;

            ClientConnection client =
                new ClientConnection(socket, roomManager);

            _ = client.ReceiveLoop();

            Console.WriteLine("Client connected");
        }
    }
}

