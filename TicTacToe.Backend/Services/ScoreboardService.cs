using Microsoft.EntityFrameworkCore;
using TicTacToe.Backend.Data;
using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Services;

public class ScoreboardService : IScoreboardService
{
    private readonly GameDbContext _context;

    public ScoreboardService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Scoreboard> GetScoreboardAsync()
    {
        var scoreboard = await _context.Scoreboards.FirstOrDefaultAsync(s => s.Id == 1);
        if (scoreboard == null)
        {
            scoreboard = new Scoreboard { Id = 1 };
            _context.Scoreboards.Add(scoreboard);
            await _context.SaveChangesAsync();
        }
        return scoreboard;
    }

    public async Task UpdateScoreAsync(Player? winner)
    {
        var scoreboard = await GetScoreboardAsync();
        
        if (winner == Player.X) scoreboard.XWins++;
        else if (winner == Player.O) scoreboard.OWins++;
        else scoreboard.Draws++;

        await _context.SaveChangesAsync();
    }

    public async Task<Scoreboard> ResetScoreboardAsync()
    {
        var scoreboard = await GetScoreboardAsync();
        scoreboard.XWins = 0;
        scoreboard.OWins = 0;
        scoreboard.Draws = 0;
        await _context.SaveChangesAsync();
        return scoreboard;
    }
}
