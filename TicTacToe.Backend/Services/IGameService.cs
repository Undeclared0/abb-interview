using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Services;

public interface IGameService
{
    Task<GameSession> CreateGameAsync(GameMode mode);
    Task<GameSession?> GetGameAsync(Guid id);
    Task<GameSession> MakeMoveAsync(Guid id, int row, int column, Player player);
    Task<GameSession> UndoLastMoveAsync(Guid id);
    Task<GameSession> ResetGameAsync(Guid id);
}
