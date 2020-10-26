namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Interfaces;
    using System.Collections.Generic;
    using System.Drawing;

    public abstract class Ship : IRepairable, IFireable
    {
        public int Id { get; set; }

        public int Range { get; set; }

        public Point Direction { get; set; }

        public Player Player { get; set; }

        public List<Deck> Decks { get; set; }

        public abstract void Fire();

        public void Move()
        {
        }

        public abstract void Repair();
    }
}
