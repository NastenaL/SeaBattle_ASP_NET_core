﻿namespace SeaBattleASP.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;

    public class ApplicationContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public DbSet<PlayingShip> PlayingShips { get; set; }

        public DbSet<Cell> Cells { get; set; }

        public DbSet<Deck> Decks { get; set; }

        public DbSet<DeckCell> DeckCells { get; set; }

        public DbSet<AuxiliaryShip> AuxiliaryShips { get; set; }

        public DbSet<MilitaryShip> MilitaryShips { get; set; }

        public DbSet<MixShip> MixShips { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayingField> PlayingFields { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayingField>().ToTable("PlayingField");
            //modelBuilder.Entity<Game>().ToTable("Game");
            //modelBuilder.Entity<DeckCell>().ToTable("DeckCell");
            //modelBuilder.Entity<MilitaryShip>().ToTable("Ship");
            //modelBuilder.Entity<AuxiliaryShip>().ToTable("Ship");
            //modelBuilder.Entity<MixShip>().ToTable("Ship");

            //modelBuilder.Entity<CourseAssignment>()
            //    .HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}
