using System;
using System.Collections.Generic;
using System.Text;

namespace TexasFuckEm.Classes
{
    public class Card
    {
        public int Value { get; init; }
        public required Suite SuiteofCard { get; init; }
        public string Face => Value switch
        {
            14 => "A",
            13 => "K",
            12 => "Q",
            11 => "J",
            _ => Value.ToString()
        };

        public override string ToString()
        {
            return SuiteofCard.ShortHand + Face;
        }
    }
}
