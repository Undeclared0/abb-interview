namespace TicTacToe.Backend.Models;

public enum Player
{
    X,
    O
}

public enum GameMode
{
    TwoPlayer,
    Computer
}

public enum GameStatus
{
    InProgress,
    Won,
    Draw,
    Expired
}
