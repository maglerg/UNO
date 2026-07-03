using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUNO
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AccountManager accountManager = new AccountManager();
            RoomManager roomManager = new RoomManager(accountManager);

            ServerManager server = new ServerManager(roomManager);
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Server is shutting down");

                accountManager.Save();
            };
            await server.Start();
        }
    }
}
