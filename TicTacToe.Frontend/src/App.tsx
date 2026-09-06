import { useEffect, useState } from 'react';
import type { GameMode, GameSession, Scoreboard } from './types';
import * as api from './api';
import './index.css';

function App() {
  const [game, setGame] = useState<GameSession | null>(null);
  const [scoreboard, setScoreboard] = useState<Scoreboard | null>(null);
  const [mode, setMode] = useState<GameMode>('TwoPlayer');

  const fetchScoreboard = async () => {
    const data = await api.getScoreboard();
    setScoreboard(data);
  };

  const initGame = async (selectedMode: GameMode) => {
    const newGame = await api.createGame(selectedMode);
    setGame(newGame);
    setMode(selectedMode);
    await fetchScoreboard();
  };

  useEffect(() => {
    initGame('TwoPlayer');
  }, []);

  const handleCellClick = async (row: number, col: number) => {
    if (!game || game.status !== 'InProgress') return;
    try {
      const updatedGame = await api.makeMove(game.id, row, col, game.currentPlayer);
      setGame(updatedGame);
      if (updatedGame.status !== 'InProgress') {
        await fetchScoreboard();
      }
    } catch (e) {
      console.error('Move failed', e);
    }
  };

  const handleUndo = async () => {
    if (!game) return;
    try {
      const updatedGame = await api.undoMove(game.id);
      setGame(updatedGame);
    } catch (e) {
      console.error('Undo failed', e);
    }
  };

  const handleReset = async () => {
    if (!game) return;
    try {
      const updatedGame = await api.resetGame(game.id);
      setGame(updatedGame);
    } catch (e) {
      console.error('Reset failed', e);
    }
  };

  const handleResetScoreboard = async () => {
    const updated = await api.resetScoreboard();
    setScoreboard(updated);
  };

  if (!game || !scoreboard) return <div className="container">Loading...</div>;

  const renderStatus = () => {
    if (game.status === 'Won') return `Winner: ${game.winner}`;
    if (game.status === 'Draw') return "It's a Draw!";
    return `Current Turn: ${game.currentPlayer}`;
  };

  return (
    <div className="container">
      <h1>Tic Tac Toe</h1>
      
      <div className="layout">
        <div className="main-panel">
          <div className="mode-selector">
            <button onClick={() => initGame('TwoPlayer')} disabled={mode === 'TwoPlayer'}>Two Player</button>
            <button onClick={() => initGame('Computer')} disabled={mode === 'Computer'}>Play Against Computer</button>
          </div>
          
          <div className="status">{renderStatus()}</div>
          
          <div className="board">
            {[0, 1, 2].map(row => 
              [0, 1, 2].map(col => {
                const index = row * 3 + col;
                const value = game.board[index];
                const isWinning = game.winningCells?.includes(index);
                return (
                  <button
                    key={index}
                    className={`cell ${isWinning ? 'winning' : ''}`}
                    onClick={() => handleCellClick(row, col)}
                    disabled={value !== null || game.status !== 'InProgress'}
                  >
                    {value}
                  </button>
                );
              })
            )}
          </div>
          
          <div className="controls">
            <button onClick={handleReset}>Reset Game</button>
            <button onClick={handleUndo} disabled={game.moves.length === 0 || game.status !== 'InProgress'}>
              Undo Last Move
            </button>
          </div>
        </div>
        
        <div className="side-panel">
          <div className="scoreboard">
            <h2>Scoreboard</h2>
            <p>X Wins: <strong>{scoreboard.xWins}</strong></p>
            <p>O Wins: <strong>{scoreboard.oWins}</strong></p>
            <p>Draws: <strong>{scoreboard.draws}</strong></p>
            <button onClick={handleResetScoreboard}>Reset Scoreboard</button>
          </div>
          
          <div>
            <h2>Move History</h2>
            {game.moves.length === 0 ? (
              <p>No moves yet.</p>
            ) : (
              <table>
                <thead>
                  <tr>
                    <th>Move</th>
                    <th>Player</th>
                    <th>Position</th>
                  </tr>
                </thead>
                <tbody>
                  {[...game.moves].sort((a,b) => a.moveNumber - b.moveNumber).map(move => (
                    <tr key={move.id || move.moveNumber}>
                      <td>{move.moveNumber}</td>
                      <td>{move.player}</td>
                      <td>Row {move.row + 1}, Col {move.column + 1}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
