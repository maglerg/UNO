using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Library
{
    public class PlayerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int CardCount { get; set; }
        public PlayerInfo() { }
        public PlayerInfo(Guid id, string name, int cardCount)
        {
            Id = id;
            Name = name;
            CardCount = cardCount;
        }
    }
}
