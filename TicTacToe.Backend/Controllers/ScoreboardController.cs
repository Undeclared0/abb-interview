using Microsoft.AspNetCore.Mvc;
using TicTacToe.Backend.Services;

namespace TicTacToe.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreboardController : ControllerBase
{
    private readonly IScoreboardService _scoreboardService;

    public ScoreboardController(IScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetScoreboard()
    {
        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        return Ok(scoreboard);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetScoreboard()
    {
        var scoreboard = await _scoreboardService.ResetScoreboardAsync();
        return Ok(scoreboard);
    }
}
