using Microsoft.EntityFrameworkCore;
using ToughBattle.Models;

namespace ToughBattle.Database
{
    public class FoosballContext : DbContext
    {
        public FoosballContext(DbContextOptions<FoosballContext> options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Matchup> Matchups { get; set; }
    }
}
