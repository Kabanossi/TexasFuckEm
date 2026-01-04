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
        static bool multiplayer;
        static int playercount;
        static List<Player> players;
        static int handCount;

        static void Main()
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scorelist.txt");
            full_list = File.ReadAllLines(filePath).ToList();
            inputY = Console.WindowHeight - 2;

            int balance = 50;

            //alkuruutu

            Console.Write("Pelaajien määrä: ");
            playercount = int.TryParse(Console.ReadLine(), out playercount) ? playercount : 1;

            if (playercount > 1)
            {
                multiplayer = true;
                players = new List<Player>();

                for (int i = 0; i < playercount; i++)
                {
                    Console.Write($"Anna pelaajan {i + 1} nimi: ");
                    Player mp = new Player() { Name = Console.ReadLine() ?? "Idiootti" };
                    players.Add(mp);
                }
            }
            else
            {
                multiplayer = false;
            }

            if (!multiplayer)
            {
                DrawStartUp(balance);
            }

            int deal_count = 1;

            if (p != null)
            {
                do
                {
                    deck = new Deck();

                    Console.Clear();

                    if (balance > 0 || deal_count == 2)
                    {

                        int voitto;


                        if (deal_count == 1)
                        {
                            bet = bet > balance ? balance : bet;
                            DrawBannerAndProfitList();

                            balance -= bet;

                            deck.Shuffle();

                            //Ekat kortit
                            p.Hand = deck.DealHand(5); //MakeTestHand(1);
                            Console.WriteLine(p.ToString());

                        }

                        if (deal_count == 2)
                        {
                            DrawBannerAndProfitList();
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
                        Console.Clear();
                        Console.WriteLine("EI RAHAA! PELI LOPPUI!");
                        break;
                    }

                    DrawStatus(balance);

                    // input aina rivillä ennen alariviä
                    string command = DrawCommandLine(deal_count, multiplayer);


                    if (deal_count == 1)
                    {//KORTTIEN VAIHTO
                        try
                        {
                            int[] discard;
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

                    DetermineCommand(command, deal_count, balance);

                    if (command == "x") break;

                } while (true);
            }
            else if (players.Count > 1 && multiplayer)
            //MULTIPLAYER
            {
                handCount = 1;
                do
                {
                    deck = new Deck();

                    Console.Clear();

                    if (deal_count == 1)
                    {
                        DrawBannerAndProfitList();
                        handCount++;

                        deck.Shuffle();

                        //Ekat kortit KAIKILLE

                        foreach (var mp in players)
                        {
                            mp.Hand = deck.DealHand(5); //MakeTestHand(1);
                            Console.WriteLine(mp.ToString());
                        }
                    }
                    if (deal_count == 2)
                    {
                        DrawBannerAndProfitList();
                        foreach (var mp in players)
                        {
                            mp.EvaluateHand();
                            Console.WriteLine($"{mp.ToString()} ({mp.CurrentHandValue})");
                        }

                    }
                    //KORTTIEN VAIHTO
                    foreach (var mp in players)
                    {
                        string command = DrawCommandLine(deal_count, multiplayer, mp);

                        if (deal_count == 1)
                        {
                            try
                            {
                                int[] discard;
                                discard = Array.ConvertAll(command.Split(','), int.Parse);

                                foreach (var i in discard.OrderByDescending(x => x))
                                {
                                    mp.Hand.RemoveAt(i - 1);
                                }

                                mp.Hand.AddRange(deck.DealHand(discard.Length));


                            }
                            catch (Exception e)
                            {

                            }
                        }

                        //DetermineCommand(command, deal_count, 0);

                        if (command == "x") break;
                    }

                    deal_count = deal_count == 1 ? 2 : 1;


                } while (true);
            }



        }

        private static void DrawStatus(int balance)
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
                case "Pari":
                    return bet * 1;

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

            if (!multiplayer)
            {
                //Voittotaulu
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(Console.WindowWidth - 19, 0);
                Console.WriteLine("  VOITTOKERTOIMET  ");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(Console.WindowWidth - 19, 1);
                Console.WriteLine(" Värisuora.....500 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 2);
                Console.WriteLine(" Neloset.......100 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 3);
                Console.WriteLine(" Täyskäsi.......50 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 4);
                Console.WriteLine(" Väri...........25 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 5);
                Console.WriteLine(" Suora..........15 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 6);
                Console.WriteLine(" Kolmoset.......10 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 7);
                Console.WriteLine(" Kaksi paria.....2 ");
                Console.SetCursorPosition(Console.WindowWidth - 19, 8);
                Console.WriteLine(" Pari............1 ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(Console.WindowWidth - 19, 10);
                Console.WriteLine($"PANOS: {bet}");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(Console.WindowWidth - 18, 1);
                Console.WriteLine($"Käsiä pelattu: {handCount}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

                Console.SetCursorPosition(0, 2);
        }
        private static void DetermineCommand(string command, int dc, int balance)
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

                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                    //panoksen vaihto
                    int possibleBet;
                    bool isnumb = int.TryParse(command, out possibleBet);

                    if (isnumb && dc == 1)
                    {
                        bet = possibleBet;
                        bet = bet > 5 ? 5 : bet;
                        bet = bet > balance ? balance : bet;
                        //deal_count = 1;
                    }
                    break;

                default:
                    //korttien vaihto

                    break;
            }
        }
        private static void DrawStartUp(int balance)
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
        private static string DrawCommandLine(int deal_count, bool mp, Player mpl = null)
        {
            Console.SetCursorPosition(0, inputY);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, inputY);
            string d1;
            string d2;
            if (mp)
            {
                d1 = $"{mpl.Name}, Valitse poistettavat kortit: ";
                d2 = "Enter: Seuraava käsi";
            }
            else
            {
                d1 = "Valitse poistettavat kortit: ";
                d2 = "Enter: Seuraava käsi | 1 - 5 : Vaihda panosta: ";
            }


            int c1 = d1.Length;
            int c2 = d2.Length;

            Console.Write(deal_count == 1 ? d1 : d2);

            return ReadInputAt(deal_count == 1 ? c1 : c2, inputY);
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