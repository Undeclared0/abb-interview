namespace TicTacToe.Backend.Models;

public class Move
{
    public int Id { get; set; }
    public Guid GameSessionId { get; set; }
    public int MoveNumber { get; set; }
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}
