using System;
using TexasFuckEm.Classes;

namespace TexasFuckEm
{
    class Program
    {
        static void Main(string[] args)
        {
            int _c = 0;

            while (true)
            {
                var deck = new Deck();

                deck.Shuffle();

                var players = new List<Player>()
            {
                new Player { Name = "Tommi"},
                new Player { Name = "Pekka"},
                new Player { Name = "Kari"}
            };



                foreach (var p in players)
                {
                    p.Hand = deck.DealHand(5);
                    _c++;
                    p.EvaluateHand();
                    Console.WriteLine($"{p.ToString()} (Value: {p.CurrentHandType} = {p.CurrentHandValue})");
                }

                if (players.Any(p => p.CurrentHandValue > 714))
                {
                    Console.WriteLine(_c);
                    break;
                }
            }

        }
    }
}