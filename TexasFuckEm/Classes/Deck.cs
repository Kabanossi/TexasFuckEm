using System;
using System.Collections.Generic;
using System.Text;

namespace TexasFuckEm.Classes
{
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = MakeaDeck();
        }

        //Metodit
        private List<Card> MakeaDeck()
        {
            List<Card> deck = new List<Card>();

            var suits = new List<Suite>();

            suits.AddRange(new Suite[]
            {
                new Suite(){Name="Hearts",ShortHand="H",Value=4},
                new Suite(){Name="Diamonds",ShortHand="D",Value=3},
                new Suite(){Name="Clubs",ShortHand = "C",Value=2},
                new Suite(){Name="Spades",ShortHand="S", Value=1}
            });

            foreach (Suite s in suits)
            {
                for (int i = 2; i < 15; i++)
                {
                    deck.Add
                    (
                        new Card() { SuiteofCard = s, Value = i }
                    );
                }
            }

            return deck;
        }
        public void Shuffle()
        {
            var r = new Random();

            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = r.Next(i + 1);


                var t = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = t;
            }
        }
        public List<Card> DealHand(int howmany)
        {
            List<Card> hand = new List<Card>();

            for (int i = 0; i < howmany; i++)
            {
                hand.Add(Cards[0]);
                Cards.RemoveAt(0);
            }

            return hand;
        }
    }
}
