namespace SeaBattleASP.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;

    public class ApplicationContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public DbSet<Deck> Decks { get; set; }

        public DbSet<AuxiliaryShip> AuxiliaryShips { get; set; }

        public DbSet<MilitaryShip> MilitaryShips { get; set; }

        public DbSet<MixShip> MixShips { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayingField> PlayingField { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

       
    }
}
