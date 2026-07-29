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
        public string Status { get; set; }
        public int AvatarId { get; set; }
        public int CardCount { get; set; }
        public bool UnoPressed { get; set; }
        public bool IsHost { get; set; }
        public PlayerInfo() { }
        public PlayerInfo(Guid id, string name, int cardCount, bool isHost, string status, int avatarId, bool unoPressed = false)
        {
            Id = id;
            Name = name;
            CardCount = cardCount;
            IsHost = isHost;
            Status = status;
            AvatarId = avatarId;
            UnoPressed = unoPressed;
        }
    }
}
