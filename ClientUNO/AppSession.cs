using System;
using System.Collections.Generic;
using System.Text;

namespace ClientUNO
{
    public class AppSession
    {
        public GameClient Client { get; }
        public GameViewModel Model { get; }

        public AppSession(GameClient client, GameViewModel model)
        {
            Client = client;
            Model = model;
        }
    }
}
