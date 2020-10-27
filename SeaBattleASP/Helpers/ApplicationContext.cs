namespace SeaBattleASP.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;

    public class ApplicationContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<PlayingField> PlayingFields { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Game>().ToTable("Game");
            modelBuilder.Entity<PlayingField>().ToTable("PlayingField");
        }
    }
}
