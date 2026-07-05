using System;
using System.Collections.Generic;

namespace Shared_Library
{
    public enum PlayerAction
    {
        None,
        Two,
        Four,
        Skip
    }
    public class Player
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public List<Card> PlayerCards { get; set; }
        public PlayerAction Action { get; set; }
        public Player() 
        {
            Id = Guid.NewGuid();
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
