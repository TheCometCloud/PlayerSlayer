using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSlayer
{
    public class Game
    {
        Player Computer { get; set; }
        Player User { get; set; }
        Stack<Card> Deck { get; set; }
        static Card Nothing = new Card() { Name = "Nothing", Priority = 10 };

        private static Random rng = new Random();

        public Game()
        {
            Initialize();
        }

        public void Initialize()
        {
            Computer = new Player("The Player Slayer");
            User = new Player("Player");

            Console.WriteLine($"Very well. Prepare to lose.");

            FillDeck();
            ShuffleDeck();
            FillHands();
            int breaths = 0;
            while (Computer.Health > 0 && User.Health > 0)
            {
                breaths++;
                DisplayState();

                if (User.Stunned)
                    ProgressTurn(0);
                else
                {
                    Console.WriteLine("Which card will you play?");
                    ProgressTurn(Int32.Parse(Console.ReadLine()));
                }

                if (breaths >= 4)
                {
                    breaths = 0;
                    ShuffleDeck();
                    FillHands();
                }
            }

            Console.WriteLine("Good game. Again?");
            if (Console.ReadLine().StartsWith("y"))
            {
                Initialize();
            }
        }

        public void FillDeck()
        {
            Deck = new Stack<Card>();

            for (int i = 0; i < 3; i++)
            {
                Deck.Push(new Card() { Name = "Roll", Priority = 2, StunAfter = false });
                Deck.Push(new Card() { Name = "Guard", Priority = 0, StunAfter = false });
                Deck.Push(new Card() { Name = "Quick Attack", Priority = 1, StunAfter = false });
                Deck.Push(new Card() { Name = "Normal Attack", Priority = 3, StunAfter = false });
                Deck.Push(new Card() { Name = "Heavy Attack", Priority = 4, StunAfter = true });
            }
        }

        public void FillHands()
        {
            while (User.Hand.Count < 5)
            {
                User.Hand.Add(Deck.Pop());
            }

            while (Computer.Hand.Count < 5)
            {
                Computer.Hand.Add(Deck.Pop());
            }
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public void ShuffleDeck()
        {
            List<Card> decklist = Deck.ToList();
            Deck = new Stack<Card>(Shuffle(decklist));
        }

        public void DisplayState()
        {
            if (User.Health > 0 && Computer.Health > 0)
            {
                Console.WriteLine($"You have {User.Health} health. I have {Computer.Health} health.");
                Console.WriteLine($"These are the cards in your hand:");
                for (int i = 0; i < User.Hand.Count; i++)
                {
                    Console.WriteLine($"[{i}]: {User.Hand[i].Name}");
                }
            }
            else if (User.Health <= 0)
            {
                Console.WriteLine("Ha! I win.");
            }
            else
            {
                Console.WriteLine("How could you win? Did you cheat?");
            }
        }

        public void ProgressTurn(int index)
        {
            bool clashed = false;

            if (index >= User.Hand.Count)
            {
                Console.WriteLine("Invalid index.");
                DisplayState();
            }

            Card cpuPlay = Computer.Stunned ? Nothing : Computer.Hand[rng.Next(Computer.Hand.Count)];
            Card userPlay = User.Stunned ? Nothing : User.Hand[index];

            Computer.Stunned = false;
            User.Stunned = false;

            if (!(cpuPlay == Nothing))
            {
                Computer.Hand.Remove(cpuPlay);
                Deck.Push(cpuPlay);
            }

            if (!(userPlay == Nothing))
            {
                User.Hand.Remove(userPlay);
                Deck.Push(userPlay);
            }

            Console.WriteLine($"You played {userPlay.Name}. I played {cpuPlay.Name}.");

            List<Card> plays = new List<Card>() { cpuPlay, userPlay };
            clashed = cpuPlay.Priority == userPlay.Priority;
            plays = plays.OrderBy(c => c.Priority).ToList();

            foreach(Card card in plays)
            {
                if (card == Nothing)
                {
                    continue;
                }

                Player target = card == cpuPlay ? User : Computer;
                Player actor = card == cpuPlay ? Computer : User;

                switch(card.Priority)
                {
                    case 0:
                        actor.Guarded = true;
                        break;
                        
                    case 1:
                        if (clashed || target.Damage(1) <= 0)
                        {
                            DisplayState();
                            return;
                        }
                        break;

                    case 2:
                        actor.Rolled = true;
                        break;

                    case 3:
                        if (clashed || target.Damage(2) <= 0)
                        {
                            DisplayState();
                            return;
                        }
                        break;

                    case 4:
                        if (clashed || target.Damage(3) <= 0)
                        {
                            DisplayState();
                            return;
                        }
                        break;
                }

                if (card.StunAfter)
                    actor.Stunned = true;
            }

            Computer.Cleanse();
            User.Cleanse();
        }
    }
}
