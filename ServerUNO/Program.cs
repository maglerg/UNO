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
            RoomManager roomManager = new RoomManager();

            ServerManager server = new ServerManager(roomManager);

            await server.Start();
        }
    }
}
