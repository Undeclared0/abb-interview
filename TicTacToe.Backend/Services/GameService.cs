using Microsoft.EntityFrameworkCore;
using TicTacToe.Backend.Data;
using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Services;

public class GameService : IGameService
{
    private readonly GameDbContext _context;
    private readonly IScoreboardService _scoreboardService;

    public GameService(GameDbContext context, IScoreboardService scoreboardService)
    {
        _context = context;
        _scoreboardService = scoreboardService;
    }

    public async Task<GameSession> CreateGameAsync(GameMode mode)
    {

        var game = new GameSession
        {
            Id = Guid.NewGuid(),
            CurrentPlayer = Player.X,
            Mode = mode,
            Status = GameStatus.InProgress
        };
        await _context.GameSessions.AddAsync(game);
        await _context.SaveChangesAsync();
        return game;
    }

    private async Task<GameSession?> GetInProgressGameAsync()
    {
        var game = await _context.GameSessions.Where(g => g.Status == GameStatus.InProgress).OrderByDescending(s => s.CreatedDate).FirstOrDefaultAsync(); 
        
        return game;
    }

    public async Task<GameSession?> GetGameAsync(Guid id)
    {
        return await _context.GameSessions
            .Include(g => g.Moves)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<GameSession> MakeMoveAsync(Guid id, int row, int column, Player player)
    {
        var game = await GetGameAsync(id);
        if (game == null) throw new ArgumentException("Game not found.");

        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game is already completed.");

        if (game.CurrentPlayer != player)
            throw new InvalidOperationException("Not your turn.");

        int cellIndex = row * 3 + column;
        if (row < 0 || row > 2 || column < 0 || column > 2 || game.Board[cellIndex] != null)
            throw new InvalidOperationException("Invalid move.");

        await ApplyMoveAsync(game, player, row, column);

        if (game.Status == GameStatus.InProgress && game.Mode == GameMode.Computer && player == Player.X)
        {
            await MakeComputerMoveAsync(game);
        }

        await _context.SaveChangesAsync();
        return game;
    }

    private async Task ApplyMoveAsync(GameSession game, Player player, int row, int column)
    {
        int cellIndex = row * 3 + column;
        game.Board[cellIndex] = player;
        
        var moveNumber = game.Moves.Count + 1;
        game.Moves.Add(new Move
        {
            GameSessionId = game.Id,
            MoveNumber = moveNumber,
            Player = player,
            Row = row,
            Column = column
        });

        CheckWinOrDraw(game);

        if (game.Status == GameStatus.InProgress)
        {
            game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;
        }
        else
        {
            await _scoreboardService.UpdateScoreAsync(game.Winner);
        }
    }

    private async Task MakeComputerMoveAsync(GameSession game)
    {
        int move = FindWinningMove(game, Player.O);
        
        if (move == -1) 
            move = FindWinningMove(game, Player.X);
        if (move == -1 && game.Board[4] == null) 
            move = 4;
        if (move == -1) 
            move = FindEmptyCorner(game);
        if (move == -1) 
            move = FindFirstEmpty(game);

        if (move != -1)
        {
            int row = move / 3;
            int col = move % 3;
            await ApplyMoveAsync(game, Player.O, row, col);
        }
    }

    private int FindWinningMove(GameSession game, Player player)
    {
        for (int i = 0; i < 9; i++)
        {
            if (game.Board[i] == null)
            {
                game.Board[i] = player;
                bool isWin = CheckWinCondition(game.Board, out _);
                game.Board[i] = null;
                if (isWin) return i;
            }
        }
        return -1;
    }

    private int FindEmptyCorner(GameSession game)
    {
        int[] corners = { 0, 2, 6, 8 };
        foreach (var c in corners)
            if (game.Board[c] == null) return c;
        return -1;
    }

    private int FindFirstEmpty(GameSession game)
    {
        for (int i = 0; i < 9; i++)
            if (game.Board[i] == null) return i;
        return -1;
    }

    private void CheckWinOrDraw(GameSession game)
    {
        if (CheckWinCondition(game.Board, out var winningCells))
        {
            game.Status = GameStatus.Won;
            game.Winner = game.CurrentPlayer;
            game.WinningCells = winningCells;
        }
        else if (!game.Board.Any(c => c == null))
        {
            game.Status = GameStatus.Draw;
        }
    }

    private bool CheckWinCondition(List<Player?> board, out List<int> winningCells)
    {
        winningCells = new List<int>();
        int[][] lines = new int[][]
        {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8},
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8},
            new[] {0, 4, 8}, new[] {2, 4, 6}
        };

        foreach (var line in lines)
        {
            if (board[line[0]] != null && 
                board[line[0]] == board[line[1]] && 
                board[line[1]] == board[line[2]])
            {
                winningCells.AddRange(line);
                return true;
            }
        }
        return false;
    }

    public async Task<GameSession> UndoLastMoveAsync(Guid id)
    {
        var game = await GetGameAsync(id);
        if (game == null) throw new ArgumentException("Game not found.");

        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Cannot undo after game is completed.");

        if (game.Moves.Count == 0)
            throw new InvalidOperationException("No moves to undo.");

        int movesToRemove = game.Mode == GameMode.Computer ? 2 : 1;
        movesToRemove = Math.Min(movesToRemove, game.Moves.Count);

        var movesList = game.Moves.OrderBy(m => m.MoveNumber).ToList();
        for (int i = 0; i < movesToRemove; i++)
        {
            var lastMove = movesList.Last();
            movesList.Remove(lastMove);
            _context.Moves.Remove(lastMove);
            game.Board[lastMove.Row * 3 + lastMove.Column] = null;
        }

        if (movesToRemove % 2 != 0)
        {
            game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;
        }
        
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<GameSession> ResetGameAsync(Guid id)
    {
        var game = await GetGameAsync(id);
        if (game == null) throw new ArgumentException("Game not found.");

        game.Board = new List<Player?>(new Player?[9]);
        game.CurrentPlayer = Player.X;
        game.Status = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells.Clear();
        
        _context.Moves.RemoveRange(game.Moves);
        await _context.SaveChangesAsync();
        return game;
    }
}
