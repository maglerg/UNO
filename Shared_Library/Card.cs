using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Library
{
    public enum ColorCard
    {
        Red, Green, Blue, Yellow, Black
    };
    public enum TypeCard
    {
        Number, Skip, Reverse, DrawTwo, Wild, WildDrawFour
    };
    public enum CardValue
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, None
    }
    public class Card
    {
        public ColorCard Color { get; set; }
        public TypeCard Type { get; set; }
        public CardValue Value { get; set; }
        public override string ToString()
        {
            return $"{Color} {Type} {Value}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Card other)
                return false;

            return Color == other.Color &&
                   Type == other.Type &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Type, Value);
        }
    }
}
