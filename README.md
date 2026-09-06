<<<<<<< HEAD
# Tic Tac Toe

## 1. Project Overview
Tic Tac Toe is a web application using React for the frontend and .NET Web API for backend. The game supports two gameplay modes. It includes features like move tracking, an undo mechanism, and a scoreboard for the game session.

## 2. Tech Stack
- **Frontend**: React.js, CSS
- **Backend**: .NET Web API, C#
- **Database**: Entity Framework Core with In-Memory Provider
- **Testing**: xUnit

## 3. Features Implemented
- 3x3 game board
- Two Player Mode
- Play against computer
- Win detection (rows, columns, diagonals)
- Draw detection
- Move History tracking
- Undo last move (removes 1 move in Two Player, 2 moves in Computer Mode)
- Session-level Scoreboard
- Premium UI with glassmorphism, gradients, and micro-animations

## 4. How to Run the Backend Locally
1. Ensure you have the .NET 8 SDK installed.
2. Navigate to the backend directory:
   ```
   cd TicTacToe.Backend
   ```
3. Run the application:
   ```
   dotnet run
   ```
   *The swagger will start on `http://localhost:5250/swagger/`.*


## 5. How to Run the Frontend Locally
1. Ensure you have Node.js and npm installed.
2. Navigate to the frontend directory:
   ```
   cd TicTacToe.Frontend
   ```
3. Install dependencies:
   ```
   npm install
   ```
4. Start the development server:
   ```
   npm run dev
   ```
   *The frontend will start and open in your default browser on `http://localhost:5173`.*

## 6. API Endpoint Summary
The backend exposes the following REST APIs:
- `POST /api/games`: Creates a new game session
- `GET /api/games/{id}`: Retrieves the current game state
- `POST /api/games/{id}/moves`: Submits a player move
- `POST /api/games/{id}/undo`: Undoes the last move(s)
- `POST /api/games/{id}/reset`: Resets the board for a new round
- `GET /api/scoreboard`: Retrieves the session scoreboard
- `POST /api/scoreboard/reset`: Resets the session scoreboard

## 7. How to Run Tests
To run the backend unit tests using xUnit:
1. Navigate to the test project directory:
   ```
   cd TicTacToe.Backend.Tests
   ```
2. Execute the tests:
   ```
   dotnet test
   ```

## 8. AI Tools and Prompt Summary
- **AI Tool**: Google Gemini 3.1 Pro via an Agentic Coding Assistant.
- **Workflow**: The requirement document was analyzed to extract key entities and rules. Clarifications were requested regarding technology stack choices (React + EF Core In-Memory) and edge case behavior (Undo after game completion). 
- **Code Generation**: The AI scaffolded the .NET API, EF Core configurations and React project as the initial project setup. It created the boilerplate for the service and controller layers. It added CSS styling for the application. Reviewed the project implementation and corner cases. It extracted a list of testing scenarios.

## 9. Design Decisions
- **Single Source of Truth**: The .NET backend retains complete logic of the game state, move validation, and scoreboard. The React frontend is purely a presentation layer.
- **EF Core In-Memory**: satisfies the requirement for an ORM without introducing the external dependency overhead of SQLite for a simple session based game.
- **UI**: Standard CSS is used to create a responsive in the React frontend.

## 10. Clarifications and Assumptions
- **Undo Behavior**: Option A in the requirements, the Undo feature is disabled once a game reaches a completed state (Win or Draw). This keeps the game rules straightforward and prevents retroactive score alterations.
- **Session Lifespan**: Because we use an In-Memory database without user authentication, the "session" applies globally to the current server runtime instance.

## 11. Known Limitations
-  in-memory database will lose all game and scoreboard data when the .NET backend is restarted.
- There is no support for concurrent multi-browser sessions, as there is no concept of a "Room Code" or user authentication yet.

## 12. Future Improvements
- **Multiplayer Support**: Introduce WebSockets for real-time remote multiplayer capabilities along with game room feature.
- **Persistent Storage**: Swap the EF Core In-Memory provider for PostgreSQL or SQL Server to persist data across server restarts.
- **Integrate AI**: Play with AI mode can be added.

=======
# abb-interview
>>>>>>> 9616b9971a96b0b73d016b425da44c301f9772ae
