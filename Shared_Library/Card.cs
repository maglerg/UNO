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
    }
}
