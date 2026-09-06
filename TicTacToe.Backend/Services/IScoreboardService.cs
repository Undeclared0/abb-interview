using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Services;

public interface IScoreboardService
{
    Task<Scoreboard> GetScoreboardAsync();
    Task UpdateScoreAsync(Player? winner);
    Task<Scoreboard> ResetScoreboardAsync();
}
