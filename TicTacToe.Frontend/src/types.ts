export type Player = 'X' | 'O';
export type GameMode = 'TwoPlayer' | 'Computer';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';

export interface Move {
    id: number;
    gameSessionId: string;
    moveNumber: number;
    player: Player;
    row: number;
    column: number;
}

export interface GameSession {
    id: string;
    currentPlayer: Player;
    mode: GameMode;
    status: GameStatus;
    winner: Player | null;
    board: (Player | null)[];
    winningCells: number[];
    moves: Move[];
}

export interface Scoreboard {
    id: number;
    xWins: number;
    oWins: number;
    draws: number;
}
