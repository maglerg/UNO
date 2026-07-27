using System;
using System.Collections.Generic;

namespace Shared_Library
{
    public enum PlayerAction
    {
        None,
        Two,
        Four,
        Skip,
        Reverse
    }
    public class Player
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public int AvatarId { get; set; }
        public string Status { get; set; }
        public List<Card> PlayerCards { get; set; }
        public PlayerAction Action { get; set; }
        public bool IsHost { get; set; }
        public Player() 
        {
            Id = Guid.NewGuid();
            PlayerCards = new List<Card>();
        }

        public Player(string name)
        {
            Id = Guid.NewGuid();
            this.Name = name;
            PlayerCards = new List<Card>();
            Action = PlayerAction.None;
        }
    }
}
