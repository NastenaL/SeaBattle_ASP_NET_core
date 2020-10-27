namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;

    public abstract class Ship : IRepairable, IFireable
    {
        public int Id { get; set; }

        public int Range { get; set; }

        public Point Direction { get; set; }

        [NotMapped]
        public Player Player { get; set; }

        public Guid PlayerId { get; set; }

        [NotMapped]
        public List<Deck> Decks { get; set; }

        public bool IsSelectedShip { get; set; }

        public abstract void Fire();

        public void Move()
        {
        }

        public abstract void Repair();
    }
}
