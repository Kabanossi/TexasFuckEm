using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TexasFuckEm.Classes;

namespace TexasFuckEm
{
    class Program
    {

        static void Main()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scorelist.txt");
            var full_list = File.ReadAllLines(filePath).ToList();

            int balance;
            int inputY = Console.WindowHeight - 2;
            int deal_count = 1;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("TOP 5 PELAAJAT:");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"{i + 1}. {full_list[i]}");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();

            Console.Write("Anna pelaajan nimi:");


            var p = new Player() { Name = Console.ReadLine() ?? "Idiootti" };

            Console.Write("Anna rahamäärä:");

            int.TryParse(Console.ReadLine(), out balance);


            if (p != null)
            {
                do
                {
                    var deck = new Deck();


                    Console.Clear();

                    Console.SetCursorPosition(0, 0);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Syöttämällä \"x\", peli loppuu");
                    Console.WriteLine();
                    Console.BackgroundColor = ConsoleColor.Black;

                    if (balance > 0 || deal_count == 2)
                    {

                        int voitto;


                        if (deal_count == 1)
                        {
                            balance--;

                            deck.Shuffle();

                            //Ekat kortit

                            p.Hand = deck.DealHand(5);
                            Console.WriteLine(p.ToString());

                        }




                        if (deal_count == 2)
                        {
                            //tokat kortit

                            Console.WriteLine(p.ToString());
                            p.EvaluateHand();
                            voitto = Profit(p.CurrentHandType);

                            if (voitto > 0)
                            {

                                balance += voitto;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{p.CurrentHandType}! Voitit {voitto} euroa!");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                Console.WriteLine("Ei voittoa.");
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("EI RAHAA! PELI LOPPUI!");
                        break;
                    }

                    DrawStatus(p.Name, balance);
                    Console.SetCursorPosition(0, inputY);

                    string d1 = "Valitse poistettavat kortit: ";
                    string d2 = "Paina Enter pelataksesi seuraavan käden.";

                    int c1 = d1.Length;
                    int c2 = d2.Length;

                    Console.Write(deal_count == 1 ? d1 : d2);


                    // input aina rivillä ennen alariviä
                    string command = ReadInputAt(deal_count == 1 ? c1 : c2, inputY);


                    if (command == "x")
                    {
                        Top_Player tp = new Top_Player();
                        tp.Name = p.Name;
                        tp.Money = balance;

                        int ranking = AddToScorelist(tp, full_list, filePath);

                        Console.Clear();
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine($"Kiitos pelaamisesta! Sinulle jäi {balance} euroa. Sijoituit sijalle {ranking}!");
                        break;
                    }



                    int[] discard;

                    if (deal_count == 1)
                    {
                        try
                        {
                            discard = Array.ConvertAll(command.Split(','), int.Parse);

                            foreach (var i in discard.OrderByDescending(x => x))
                            {
                                p.Hand.RemoveAt(i - 1);
                            }

                            p.Hand.AddRange(deck.DealHand(discard.Length));

                            deal_count = 2;
                        }
                        catch (Exception e)
                        {
                            deal_count = 2;
                        }
                    }
                    else
                    {
                        deal_count = 1;
                    }


                } while (true);
            }



        }

        private static void DrawStatus(string player, int balance)
        {
            int y = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, y);
            Console.Write($"|{player}|Rahaa: {balance}|");
        }
        private static int Profit(string type)
        {
            switch (type)
            {
                case "Neloset":
                    return 100;

                case "Täyskäsi":
                    return 10;

                case "Kolmoset":
                    return 5;

                case "Kaksi paria":
                    return 2;

                default:
                    return 0;
            }
        }
        private static string ReadInputAt(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.CursorVisible = true;

            var input = "";

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.SetCursorPosition(x + input.Length, y);
                    Console.Write(" ");
                    Console.SetCursorPosition(x + input.Length, y);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            Console.CursorVisible = false;
            return input;
        }
        private static int AddToScorelist(Top_Player player, List<string> full_list, string filepath)
        {
            List<Top_Player> top_Players = new List<Top_Player>();

            top_Players.Add(player);

            foreach (var x in full_list)
            {
                var match = Regex.Match(x, @"^(.*),\s*(\d+)\s*euroa$");

                string n = match.Groups[1].Value;
                int m = int.Parse(match.Groups[2].Value);

                top_Players.Add(new Top_Player { Money = m, Name = n });
            }

            top_Players = top_Players.OrderByDescending(x => x.Money).ToList();

            int standing = top_Players.IndexOf(player) + 1;

            var lines = new List<string>();

            for (int i = 0; i < top_Players.Count; i++)
            {
                lines.Add($"{top_Players[i].Name}, {top_Players[i].Money} euroa");
            }

            if (File.Exists(filepath))
            {

                File.WriteAllLines(filepath, lines);
            }
            return standing;
        }



    }
}