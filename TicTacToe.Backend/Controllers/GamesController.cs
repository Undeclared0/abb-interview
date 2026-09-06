using Microsoft.AspNetCore.Mvc;
using TicTacToe.Backend.Models;
using TicTacToe.Backend.Services;

namespace TicTacToe.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        var game = await _gameService.CreateGameAsync(request.Mode);
        return Ok(game);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGame(Guid id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null) return NotFound();
        return Ok(game);
    }

    [HttpPost("{id}/moves")]
    public async Task<IActionResult> MakeMove(Guid id, [FromBody] MakeMoveRequest request)
    {
        try
        {
            var game = await _gameService.MakeMoveAsync(id, request.Row, request.Column, request.Player);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/undo")]
    public async Task<IActionResult> UndoMove(Guid id)
    {
        try
        {
            var game = await _gameService.UndoLastMoveAsync(id);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/reset")]
    public async Task<IActionResult> ResetGame(Guid id)
    {
        try
        {
            var game = await _gameService.ResetGameAsync(id);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class CreateGameRequest { public GameMode Mode { get; set; } }
public class MakeMoveRequest { public int Row { get; set; } public int Column { get; set; } public Player Player { get; set; } }
