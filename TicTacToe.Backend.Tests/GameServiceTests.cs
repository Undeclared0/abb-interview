using Microsoft.EntityFrameworkCore;
using TicTacToe.Backend.Data;
using TicTacToe.Backend.Models;
using TicTacToe.Backend.Services;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Backend.Tests;

public class GameServiceTests
{
    private GameDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new GameDbContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    private GameService CreateGameService(GameDbContext context)
    {
        var scoreboardService = new ScoreboardService(context);
        return new GameService(context, scoreboardService);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldCreateGameSession()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);

        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        Assert.NotNull(game);
        Assert.Equal(GameMode.TwoPlayer, game.Mode);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public async Task MakeMoveAsync_ValidMove_ShouldUpdateBoardAndSwitchTurn()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        var updated = await service.GetGameAsync(game.Id);

        Assert.Equal(Player.X, updated.Board[0]);
        Assert.Equal(Player.O, updated.CurrentPlayer);
        Assert.Single(updated.Moves);
    }

    [Fact]
    public async Task MakeMoveAsync_InvalidMove_OccupiedCell_ShouldThrow()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.MakeMoveAsync(game.Id, 0, 0, Player.O));
    }

    [Fact]
    public async Task MakeMoveAsync_InvalidMove_WrongTurn_ShouldThrow()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.MakeMoveAsync(game.Id, 0, 0, Player.O));
    }

    [Fact]
    public async Task MakeMoveAsync_RowWin_ShouldDetectWinAndUpdateScoreboard()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 0, Player.O);
        await service.MakeMoveAsync(game.Id, 0, 1, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 0, 2, Player.X);

        var updated = await service.GetGameAsync(game.Id);
        Assert.Equal(GameStatus.Won, updated.Status);
        Assert.Equal(Player.X, updated.Winner);
        Assert.Contains(0, updated.WinningCells);
        Assert.Contains(1, updated.WinningCells);
        Assert.Contains(2, updated.WinningCells);

        var scoreboard = await context.Scoreboards.FirstAsync();
        Assert.Equal(1, scoreboard.XWins);
    }

    [Fact]
    public async Task MakeMoveAsync_ColumnWin_ShouldDetectWin()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 0, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 1, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 2, 0, Player.X);

        var updated = await service.GetGameAsync(game.Id);
        Assert.Equal(GameStatus.Won, updated.Status);
        Assert.Contains(0, updated.WinningCells);
        Assert.Contains(3, updated.WinningCells);
        Assert.Contains(6, updated.WinningCells);
    }

    [Fact]
    public async Task MakeMoveAsync_DiagonalWin_ShouldDetectWin()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 0, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 1, 1, Player.X);
        await service.MakeMoveAsync(game.Id, 0, 2, Player.O);
        await service.MakeMoveAsync(game.Id, 2, 2, Player.X);

        var updated = await service.GetGameAsync(game.Id);
        Assert.Equal(GameStatus.Won, updated.Status);
        Assert.Contains(0, updated.WinningCells);
        Assert.Contains(4, updated.WinningCells);
        Assert.Contains(8, updated.WinningCells);
    }

    [Fact]
    public async Task MakeMoveAsync_Draw_ShouldDetectDraw()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 0, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 0, 2, Player.X);

        await service.MakeMoveAsync(game.Id, 1, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 1, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 2, 0, Player.O);
        
        await service.MakeMoveAsync(game.Id, 1, 2, Player.X);
        await service.MakeMoveAsync(game.Id, 2, 2, Player.O);
        await service.MakeMoveAsync(game.Id, 2, 1, Player.X);

        var updated = await service.GetGameAsync(game.Id);
        Assert.Equal(GameStatus.Draw, updated.Status);
        Assert.Null(updated.Winner);

        var scoreboard = await context.Scoreboards.FirstAsync();
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public async Task ResetGameAsync_ShouldClearBoardButKeepId()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        var resetGame = await service.ResetGameAsync(game.Id);

        Assert.Equal(game.Id, resetGame.Id);
        Assert.Empty(resetGame.Moves);
        Assert.All(resetGame.Board, cell => Assert.Null(cell));
        Assert.Equal(Player.X, resetGame.CurrentPlayer);
    }

    [Fact]
    public async Task UndoLastMoveAsync_TwoPlayerMode_ShouldRemoveOneMove()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 1, Player.O);

        var undoneGame = await service.UndoLastMoveAsync(game.Id);

        Assert.Single(undoneGame.Moves);
        Assert.Equal(Player.O, undoneGame.CurrentPlayer);
        Assert.Null(undoneGame.Board[4]); 
        Assert.Equal(Player.X, undoneGame.Board[0]); 
    }

    [Fact]
    public async Task UndoLastMoveAsync_ComputerMode_ShouldRemoveTwoMoves()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.Computer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        
        var undoneGame = await service.UndoLastMoveAsync(game.Id);

        Assert.Empty(undoneGame.Moves);
        Assert.Equal(Player.X, undoneGame.CurrentPlayer);
        Assert.All(undoneGame.Board, cell => Assert.Null(cell));
    }

    [Fact]
    public async Task Scoreboard_Update_ShouldUpdateScoreCorrectly()
    {
        var context = GetDatabaseContext();
        var scoreboardService = new ScoreboardService(context);

        await scoreboardService.UpdateScoreAsync(Player.X);
        await scoreboardService.UpdateScoreAsync(Player.O);
        await scoreboardService.UpdateScoreAsync(null);

        var scoreboard = await scoreboardService.GetScoreboardAsync();
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public async Task MakeMoveAsync_ComputerMode_ShouldSelectValidMove()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.Computer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        
        var updated = await service.GetGameAsync(game.Id);
        
        Assert.Equal(Player.O, updated.Board[4]);
        Assert.Equal(2, updated.Moves.Count);
        Assert.Equal(Player.X, updated.CurrentPlayer); 
    }

    [Fact]
    public async Task MakeMoveAsync_AfterGameCompletion_ShouldThrow()
    {
        var context = GetDatabaseContext();
        var service = CreateGameService(context);
        var game = await service.CreateGameAsync(GameMode.TwoPlayer);

        await service.MakeMoveAsync(game.Id, 0, 0, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 0, Player.O);
        await service.MakeMoveAsync(game.Id, 0, 1, Player.X);
        await service.MakeMoveAsync(game.Id, 1, 1, Player.O);
        await service.MakeMoveAsync(game.Id, 0, 2, Player.X);

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.MakeMoveAsync(game.Id, 2, 2, Player.O));
    }
}
