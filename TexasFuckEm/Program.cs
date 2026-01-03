using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TexasFuckEm.Classes;

namespace TexasFuckEm
{
    class Program
    {

        static Player p;
        static string filePath;
        static List<string> full_list;
        static int bet;
        static int inputY;
        static Deck deck;
        static void Main()
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scorelist.txt");
            full_list = File.ReadAllLines(filePath).ToList();
            inputY = Console.WindowHeight - 2;
            int balance = 50;

            //alkuruutu
            DrawStartUp();
            
            int deal_count = 1;

            if (p != null)
            {
                do
                {
                    deck = new Deck();

                    //yläbanner ja voittotaulu
                    Console.Clear();
                    DrawBannerAndProfitList();


                    if (balance > 0 || deal_count == 2)
                    {

                        int voitto;


                        if (deal_count == 1)
                        {

                            balance -= bet;

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

                    DrawStatus();
                    Console.SetCursorPosition(0, inputY);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, inputY);
                    string d1 = "Valitse poistettavat kortit: ";
                    string d2 = "Enter: Seuraava käsi k 1 - 5 : Vaihda panosta: ";

                    int c1 = d1.Length;
                    int c2 = d2.Length;

                    Console.Write(deal_count == 1 ? d1 : d2);

                    // input aina rivillä ennen alariviä
                    string command = ReadInputAt(deal_count == 1 ? c1 : c2, inputY);



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

                    DetermineCommand(command, deal_count);

                    if (command == "x") break;

                } while (true);
            }



        }

        private static void DrawStatus()
        {
            int y = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, y);
            Console.Write($"|{p.Name}|Rahaa: {balance}|");
        }
        private static int Profit(string type)
        {
            switch (type)
            {
                case "Värisuora":
                    return bet * 500;

                case "Neloset":
                    return bet * 100;

                case "Täyskäsi":
                    return bet * 50;

                case "Väri":
                    return bet * 25;

                case "Suora":
                    return bet * 15;

                case "Kolmoset":
                    return bet * 10;

                case "Kaksi paria":
                    return bet * 2;

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

            if (standing > 10)
            {
                return -1;
            }
            else
            {
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
        private static void DrawBannerAndProfitList()
        {
            //Yläpaneeli
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Syöttämällä \"x\" missä tahansa vaiheessa, peli loppuu");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;

            //Voittotaulu
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(Console.WindowWidth - 17, 0);
            Console.WriteLine("Värisuora.....500");
            Console.SetCursorPosition(Console.WindowWidth - 17, 1);
            Console.WriteLine("Neloset.......100");
            Console.SetCursorPosition(Console.WindowWidth - 17, 2);
            Console.WriteLine("Täyskäsi.......50");
            Console.SetCursorPosition(Console.WindowWidth - 17, 3);
            Console.WriteLine("Väri...........25");
            Console.SetCursorPosition(Console.WindowWidth - 17, 4);
            Console.WriteLine("Suora..........15");
            Console.SetCursorPosition(Console.WindowWidth - 17, 5);
            Console.WriteLine("Kolmoset.......10");
            Console.SetCursorPosition(Console.WindowWidth - 17, 6);
            Console.WriteLine("Kaksi paria.....2");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth - 17, 7);
            Console.WriteLine($"PANOS: {bet}");

            Console.SetCursorPosition(0, 2);
        }
        private static void DetermineCommand(string command, int dc)
        {
            switch (command)
            {
                case "x":
                    //lopetus
                    Top_Player tp = new Top_Player();
                    tp.Name = p.Name;
                    tp.Money = balance;

                    int ranking = AddToScorelist(tp, full_list, filePath);

                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Kiitos pelaamisesta! Sinulle jäi {balance} euroa. {(ranking > 0 ? $"Sijoituit sijalle {ranking}!" : "Et päässyt Top 10:n.")}");
                    break;

                case "1":case "2":case "3": case "4":case "5":
                    //panoksen vaihto
                    int possibleBet;
                    bool isnumb = int.TryParse(command, out possibleBet);

                    if (isnumb && dc == 1)
                    {
                        bet = possibleBet;
                        bet = bet > 5 ? 5 : bet;
                        //deal_count = 1;
                    }
                    break;

                default:
                    //korttien vaihto

                    break;
            }
        }
        private static void DrawStartUp()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("TOP 10 PELAAJAT:");

            for (int i = 0; i < full_list.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {full_list[i]}");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();

            Console.Write("Anna pelaajan nimi: ");


            p = new Player() { Name = Console.ReadLine() ?? "Idiootti" };
            p.Name = p.Name == "" ? "Idiootti" : p.Name;

            Console.WriteLine($"Rahamäärä: {balance}");

            //int.TryParse(Console.ReadLine(), out balance);
            

            Console.Write("Anna panos (1 - 5) ja aloita: ");
            bet = int.TryParse(Console.ReadLine(), out bet) ? bet : 1;
            bet = bet > 5 ? 5 : bet;
        }


        //Metodeja testaukseen
        /// <summary>
        /// 1:Värisuora
        /// 2:Väri
        /// 3:Suora
        /// </summary>
        /// <param name="typeofhand"></param>
        /// <returns>Korttilistan</returns>
        private static List<Card> MakeTestHand(int typeofhand)
        {
            List<Card> ret = new List<Card>();

            switch (typeofhand)
            {
                case 1:
                    for (int i = 0; i < 5; i++)
                    {
                        ret.Add(
                            new Card { SuiteofCard = new Suite() { Name = "Hearts", ShortHand = "H", Value = 4 }, Value = i + 3 }
                            );
                    }
                    break;

                case 2:
                    for (int i = 0; i < 4; i++)
                    {
                        ret.Add(
                            new Card { SuiteofCard = new Suite() { Name = "Hearts", ShortHand = "H", Value = 4 }, Value = i + 3 }
                            );
                    }
                    ret.Add(
                        new Card { SuiteofCard = new Suite() { Name = "Hearts", ShortHand = "H", Value = 4 }, Value = 10 }
                        );
                    break;

                case 3:
                    for (int i = 0; i < 4; i++)
                    {
                        ret.Add(
                            new Card { SuiteofCard = new Suite() { Name = "Hearts", ShortHand = "H", Value = 4 }, Value = i + 3 }
                            );
                    }
                    ret.Add(
                        new Card { SuiteofCard = new Suite() { Name = "Spades", ShortHand = "S", Value = 4 }, Value = 7 }
                        );
                    break;
            }

            return ret;
        }



    }
}