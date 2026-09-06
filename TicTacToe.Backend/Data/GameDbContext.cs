using Microsoft.EntityFrameworkCore;
using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Scoreboard> Scoreboards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameSession>()
            .HasMany(g => g.Moves)
            .WithOne()
            .HasForeignKey(m => m.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Scoreboard>().HasData(new Scoreboard { Id = 1, XWins = 0, OWins = 0, Draws = 0 });
    }
}
