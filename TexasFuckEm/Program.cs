using System;
using TexasFuckEm.Classes;

namespace TexasFuckEm
{
    class Program
    {

        static void Main()
        {
            int balance;

            Console.Write("Anna pelaajan nimi:");


            var p = new Player() { Name = Console.ReadLine() ?? "Idiootti" };

            Console.Write("Anna rahamäärä:");

            int.TryParse(Console.ReadLine(), out balance);



            if (p != null)
            {
                do
                {

                    int deal_count = 1;
                    //Console.CursorVisible = false;

                    Console.Clear();



                    Console.SetCursorPosition(0, 0);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Syöttämällä \"x\", peli loppuu");
                    Console.WriteLine();
                    Console.BackgroundColor = ConsoleColor.Black;

                    if (balance > 0)
                    {

                        int voitto;
                        var deck = new Deck();

                        deck.Shuffle();

                        //Ekat kortit
                        balance--;
                        p.Hand = deck.DealHand(5);  
                        Console.WriteLine(p.ToString());

                        string komento = Console.ReadLine();


                        if (deal_count == 2)
                        {
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
                        Console.WriteLine("EI RAHAA!");
                    }

                    DrawStatus(p.Name, balance);



                    // input aina rivillä ennen statusriviä
                    int inputY = Console.WindowHeight - 2;
                    Console.SetCursorPosition(0, inputY);
                    Console.Write("Komento: ");

                    string command = ReadInputAt(9, inputY);

                    if (command == "x")
                        break;

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

        static string ReadInputAt(int x, int y)
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


    }
}