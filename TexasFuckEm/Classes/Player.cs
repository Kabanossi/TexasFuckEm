using System;
using System.Collections.Generic;
using System.Text;

namespace TexasFuckEm.Classes
{
    public class Player
    {
        public required string Name { get; set; }
        public List<Card>? Hand { get; set; }
        public double? CurrentHandValue { get; set; }
        public string? CurrentHandType { get; set; }
        public int MpPoints { get; set; }

        public Player()
        {

        }

        //Metodit
        public void EvaluateHand()
        {
            var counts = Hand.GroupBy(x => x.Value).Select(g => g.Count()).OrderByDescending(c => c).ToArray();


            string type;
            if (Hand.All(c => c.SuiteofCard.Name == Hand[0].SuiteofCard.Name)) type = "Väri";
            else if (IsStraight(Hand)) type = "Suora";
            else if (counts[0] == 4) type = "Neloset";
            else if (counts[0] == 3 && counts[1] == 2) type = "Täyskäsi";
            else if (counts[0] == 3) type = "Kolmoset";
            else if (counts[0] == 2 && counts[1] == 2) type = "Kaksi paria";
            else if (counts[0] == 2) type = "Pari";
            else type = "Korkein kortti";

            CurrentHandType = type;

            switch (type)
            {
                case "Neloset":
                    int four = Hand.GroupBy(x => x.Value).First(g => g.Count() == 4).Key;
                    int kicker = Hand.GroupBy(x => x.Value).First(g => g.Count() == 1).Key;
                    CurrentHandValue = 700 + four + kicker / 100.0;
                    break;

                case "Täyskäsi":
                    int three = Hand.GroupBy(x => x.Value).First(g => g.Count() == 3).Key;
                    int pair = Hand.GroupBy(x => x.Value).First(g => g.Count() == 2).Key;
                    CurrentHandValue = 600 + three + pair / 100.0;
                    break;

                case "Väri":
                    if (IsStraight(Hand))
                    {
                        var points = Hand.OrderByDescending(x => x.Value).ToArray();
                        CurrentHandValue = 800 + Hand.FirstOrDefault().SuiteofCard.Value +
                                                 points[0].Value / 100.0 +
                                                 points[1].Value / 10000.0 +
                                                 points[2].Value / 1000000.0 +
                                                 points[3].Value / 100000000.0 +
                                                 points[4].Value / 10000000000.0;

                        CurrentHandType = "Värisuora";
                    }
                    else
                    {
                        var points = Hand.OrderByDescending(x => x.Value).ToArray();
                        CurrentHandValue = 500 + Hand.FirstOrDefault().SuiteofCard.Value +
                                                 points[0].Value / 100.0 + 
                                                 points[1].Value / 10000.0 + 
                                                 points[2].Value / 1000000.0 + 
                                                 points[3].Value / 100000000.0 + 
                                                 points[4].Value / 10000000000.0;

                    }
                    break;

                case "Suora":
                    var points2 = Hand.OrderByDescending(x => x.Value).ToArray();
                    CurrentHandValue = 400 + points2[0].Value +
                                             points2[1].Value / 100.0 +
                                             points2[2].Value / 10000.0 +
                                             points2[3].Value / 1000000.0 +
                                             points2[4].Value / 100000000.0;
                    break;

                case "Kolmoset":
                    three = Hand.GroupBy(x => x.Value).First(g => g.Count() == 3).Key;//esim jätkä on 11
                    var kickers = Hand.Where(x => x.Value != three).OrderByDescending(x => x.Value).ToArray();
                    CurrentHandValue = 300 + three + kickers[0].Value / 100.0 + kickers[1].Value / 10000.0;
                    break;

                case "Kaksi paria":
                    var pairs = Hand.GroupBy(x => x.Value).Where(g => g.Count() == 2).Select(g => g.Key).OrderByDescending(x => x).ToArray();
                    kicker = Hand.GroupBy(x => x.Value).First(g => g.Count() == 1).Key;
                    CurrentHandValue = 200 + pairs[0] + pairs[1] / 100.0 + kicker / 10000.0;
                    break;

                case "Pari":
                    pair = Hand.GroupBy(x => x.Value).First(g => g.Count() == 2).Key;
                    kickers = Hand.Where(x => x.Value != pair).OrderByDescending(x => x.Value).ToArray();
                    CurrentHandValue = 100 + pair + kickers[0].Value / 100.0 + kickers[1].Value / 10000.0 + kickers[2].Value / 1000000.0;
                    break;

                case "Korkein kortti":
                    var values = Hand.OrderByDescending(x => x.Value).ToArray();
                    CurrentHandValue = values[0].Value + values[1].Value / 100.0 + values[2].Value / 10000.0 + values[3].Value / 1000000.0 + values[4].Value / 100000000.0;
                    break;
            }

        }
        public override string ToString()
        {
            return $"{Name}: {string.Join(' ', Hand)}";
        }

        private bool IsStraight(List<Card> hand)
        {
            var values = hand
                            .Select(c => c.Value)
                            .Distinct()
                            .OrderBy(v => v)
                            .ToList();

            if (values.Count != 5)
                return false;

            // Normaali suora
            bool normalStraight = values[4] - values[0] == 4;

            // A-2-3-4-5
            bool wheelStraight = values.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 });

            return normalStraight || wheelStraight;
        }
    }
}
