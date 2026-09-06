import type { GameMode, GameSession, Player, Scoreboard } from './types';

const API_BASE_URL = 'http://localhost:5250/api'

export const createGame = async (mode: GameMode): Promise<GameSession> => {
    const response = await fetch(`${API_BASE_URL}/games`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ mode })
    });
    return response.json();
};

export const getGame = async (id: string): Promise<GameSession> => {
    const response = await fetch(`${API_BASE_URL}/games/${id}`);
    return response.json();
};

export const makeMove = async (id: string, row: number, column: number, player: Player): Promise<GameSession> => {
    const response = await fetch(`${API_BASE_URL}/games/${id}/moves`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ row, column, player })
    });
    if (!response.ok) throw new Error(await response.text());
    return response.json();
};

export const undoMove = async (id: string): Promise<GameSession> => {
    const response = await fetch(`${API_BASE_URL}/games/${id}/undo`, { method: 'POST' });
    if (!response.ok) throw new Error(await response.text());
    return response.json();
};

export const resetGame = async (id: string): Promise<GameSession> => {
    const response = await fetch(`${API_BASE_URL}/games/${id}/reset`, { method: 'POST' });
    if (!response.ok) throw new Error(await response.text());
    return response.json();
};

export const getScoreboard = async (): Promise<Scoreboard> => {
    const response = await fetch(`${API_BASE_URL}/scoreboard`);
    return response.json();
};

export const resetScoreboard = async (): Promise<Scoreboard> => {
    const response = await fetch(`${API_BASE_URL}/scoreboard/reset`, { method: 'POST' });
    return response.json();
};
