using System;

namespace Shared_Library
{
    public class Deck
    {
        public List<Card> Cards { get; set; }
        public Deck()
        {
            Cards = new List<Card>();
        }
        private static readonly Random random = new();
        public void CreateDeck()
        {
            Cards.Clear();

            ColorCard[] colors =
            {
                ColorCard.Red,
                ColorCard.Green,
                ColorCard.Blue,
                ColorCard.Yellow
            };
            TypeCard[] types =
            {
                TypeCard.Skip, 
                TypeCard.Reverse,
                TypeCard.DrawTwo
            };
            TypeCard[] typesblack =
            {
                TypeCard.Wild,
                TypeCard.WildDrawFour
            };
            foreach (ColorCard color in colors)
            {
                Cards.Add(new Card
                {
                    Color = color,
                    Type = TypeCard.Number,
                    Value = CardValue.Zero
                });

                for (byte i = 1; i <= 9; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Cards.Add(new Card
                        {
                            Color = color,
                            Type = TypeCard.Number,
                            Value = (CardValue)i
                        });
                    }
                }
                foreach (TypeCard type in types)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Cards.Add(new Card
                        {
                            Color = color,
                            Type = type,
                            Value = CardValue.None
                        });
                    }
                    
                }
            }
            foreach(TypeCard type in typesblack)
            {
                for (int i = 0; i < 4; i++)
                {
                    Cards.Add(new Card
                    {
                        Color = ColorCard.Black,
                        Type = type,
                        Value = CardValue.None
                    });
                }
            }
            Cards = Cards.OrderBy(x => random.Next()).ToList();
        }
        public Card GiveCard()
        {
            if (Cards.Count == 0)
            {
                throw new InvalidOperationException("Deck is empty");
            }

            Card card = Cards[0];

            Cards.RemoveAt(0);

            return card;
        }
    }
    
}
