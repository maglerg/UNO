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
        public string Name { get; set; }
        public List<Card> PlayerCards { get; set; }
        public PlayerAction Action { get; set; }

        public Player(string name)
        {
            this.Name = name;
            PlayerCards = new List<Card>();
            Action = PlayerAction.None;
        }
    }
}
