namespace TicTacToe.Backend.Models;

public class GameSession
{
    public Guid Id { get; set; }
    public Player CurrentPlayer { get; set; }
    public GameMode Mode { get; set; }
    public GameStatus Status { get; set; }
    public Player? Winner { get; set; }
    
    public List<Player?> Board { get; set; } = new List<Player?>(new Player?[9]);
    
    public List<int> WinningCells { get; set; } = new List<int>();

    public ICollection<Move> Moves { get; set; } = new List<Move>();
    public DateTime CreatedDate {get; set;} = DateTime.UtcNow;
}
