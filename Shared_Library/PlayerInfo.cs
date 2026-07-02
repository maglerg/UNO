using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Library
{
    public class PlayerInfo
    {
        public string Name { get; set; }

        public int CardCount { get; set; }

        public PlayerInfo(string name, int cardCount)
        {
            Name = name;
            CardCount = cardCount;
        }
    }
}
