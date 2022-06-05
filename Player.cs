using System;
using System.Collections.Generic;

namespace PlayerSlayer
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public int Health { get; set; }
        public bool Stunned { get; set; }
        public bool Guarded { get; set; }
        public bool Rolled { get; set; }

        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
            Health = 3;
            Stunned = false;
            Guarded = false;
            Rolled = false;
        }

        public int Damage(int damage)
        {
            if (Guarded)
                damage -= 1;

            if (Rolled)
                damage = 0;

            if (damage > 0)
                Health -= damage;

            return Health;
        }

        public void Cleanse()
        {
            Guarded = false;
            Rolled = false;
        }
    }
}
