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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AvatarId { get; set; }
        public string Status { get; set; }
        public List<Card> PlayerCards { get; set; }
        public PlayerAction Action { get; set; }
        public bool IsHost { get; set; }
        public bool UnoPressed { get; set; }
        public int Wins { get; set; }
        public int CountGames { get; set; }
        public int Points { get; set; }
        public Player() 
        {
            Id = Guid.NewGuid();
            PlayerCards = new List<Card>();
            UnoPressed = false;
        }

        public Player(string name)
        {
            Id = Guid.NewGuid();
            this.Name = name;
            PlayerCards = new List<Card>();
            Action = PlayerAction.None;
            UnoPressed = false;
        }
    }
}
