using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages;

public class GameModel : PageModel
{
    private readonly IRepository<GameState> _gameRepository;

    public GameModel(IRepository<GameState> gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public GameBrain Brain { get; private set; } = default!;
    public GameState GameState { get; private set; } = default!;
    public ECellState? Winner { get; private set; }
    public bool IsDraw { get; private set; }
    public bool IsAiTurn { get; private set; }
    public bool IsCvCMode { get; private set; }
    public string CurrentPlayerName { get; private set; } = "";
    public string GameMessage { get; private set; } = "";
    public int? LastMoveColumn { get; private set; }
    public int? LastMoveRow { get; private set; }

    public IActionResult OnGet()
    {
        // Load game state from repository
        var state = _gameRepository.Load(Id);
        if (state == null)
        {
            return RedirectToPage("/Index");
        }

        SetupGame(state);
        return Page();
    }

    public IActionResult OnPostMove(int column)
    {
        var state = _gameRepository.Load(Id);
        if (state == null) return RedirectToPage("/Index");

        SetupGame(state);
        
        // Check if game already over - REJECT move if so
        if (Winner != null || IsDraw) return RedirectToPage("/Game", new { Id });

        // Calculate and execute move
        var moveResult = Brain.CalculateMove(column);
        if (moveResult.IsValid)
        {
            Brain.ExecuteMove(moveResult.Column, moveResult.FinalRow);
            
            // Track last move for animation
            LastMoveColumn = column;
            LastMoveRow = moveResult.FinalRow;
            
            // Check winner immediately after move
            var winner = Brain.MarkWinner();
            if (winner == ECellState.XWin || winner == ECellState.OWin)
            {
                GameState = Brain.GetGameState();
                _gameRepository.Save(GameState);
                SetupGame(GameState);
                return Page();
            }
            SaveAndRefresh();
        }

        return Page();
    }

    public IActionResult OnPostAiMove()
    {
        var state = _gameRepository.Load(Id);
        if (state == null) return RedirectToPage("/Index");

        SetupGame(state);
        
        // Check if game already over or not AI's turn - REJECT move if so
        if (Winner != null || IsDraw || !IsAiTurn) return RedirectToPage("/Game", new { Id });

        // Execute AI move
        var config = state.Configuration ?? new GameConfiguration();
        
        // In PvC mode, AI is always O (not X)
        // In CvC mode, AI plays X when it's X's turn, O when it's O's turn
        bool isPlayerX = state.GameMode == EGameMode.CvC && Brain.IsNextPlayerX();
        var ai = new MinimaxAI(config, isPlayerX);
        int bestColumn = ai.GetBestMove(Brain.GetBoard(), Brain.IsNextPlayerX());
        
        var moveResult = Brain.CalculateMove(bestColumn);
        if (moveResult.IsValid)
        {
            Brain.ExecuteMove(moveResult.Column, moveResult.FinalRow);
            
            // Track last move for animation
            LastMoveColumn = bestColumn;
            LastMoveRow = moveResult.FinalRow;
            
            // Check winner immediately after move
            var winner = Brain.MarkWinner();
            if (winner == ECellState.XWin || winner == ECellState.OWin)
            {
                GameState = Brain.GetGameState();
                _gameRepository.Save(GameState);
                SetupGame(GameState);
                return Page();
            }
            SaveAndRefresh();
        }

        return Page();
    }

    public IActionResult OnPostSave(string saveName)
    {
        var state = _gameRepository.Load(Id);
        if (state == null) return RedirectToPage("/Index");

        state.SaveName = saveName;
        _gameRepository.Save(state);
        
        SetupGame(state);
        return Page();
    }

    private void SetupGame(GameState state)
    {
        GameState = state;
        
        // Create GameBrain from game state and load board
        Brain = new GameBrain(state);
        Brain.LoadGameState(state);
        
        // Check for winner by scanning all cells
        Winner = FindWinner();
        IsDraw = Winner == null && Brain.IsBoardFull();
        
        bool isXTurn = Brain.IsNextPlayerX();
        
        // Use player names from GameState (fall back to defaults if empty)
        CurrentPlayerName = isXTurn 
            ? (string.IsNullOrEmpty(state.P1Name) ? "Player 1" : state.P1Name)
            : (string.IsNullOrEmpty(state.P2Name) ? "Player 2" : state.P2Name);
        
        // Check if it's AI's turn (PvC mode and O's turn, or CvC mode always)
        IsAiTurn = (state.GameMode == EGameMode.PvC && !isXTurn) || state.GameMode == EGameMode.CvC;
        IsCvCMode = state.GameMode == EGameMode.CvC;
        
        // Set game message
        if (Winner == ECellState.X || Winner == ECellState.XWin)
            GameMessage = $"{(string.IsNullOrEmpty(state.P1Name) ? "Player 1" : state.P1Name)} Wins!";
        else if (Winner == ECellState.O || Winner == ECellState.OWin)
            GameMessage = $"{(string.IsNullOrEmpty(state.P2Name) ? "Player 2" : state.P2Name)} Wins!";
        else if (IsDraw)
            GameMessage = "Game is a Draw!";
        else
            GameMessage = $"{CurrentPlayerName}'s Turn";
    }

    private ECellState? FindWinner()
    {
        var board = Brain.GetBoard();
        int width = board.GetLength(0);
        int height = board.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == ECellState.XWin)
                    return ECellState.XWin;
                if (board[x, y] == ECellState.OWin)
                    return ECellState.OWin;
            }
        }
        return null;
    }

    private void SaveAndRefresh()
    {
        // Load the existing game state from repository to ensure proper tracking
        var existingState = _gameRepository.Load(Id);
        if (existingState == null) return;
        
        // Update only the board state and turn, keeping player names and foreign key relationships intact
        var currentGameState = Brain.GetGameState();
        existingState.Board = currentGameState.Board;
        existingState.NextMoveByX = currentGameState.NextMoveByX;
        existingState.P1Name = currentGameState.P1Name;
        existingState.P2Name = currentGameState.P2Name;
        existingState.GameMode = currentGameState.GameMode;
        
        // Save the updated state (Configuration relationship is preserved)
        _gameRepository.Save(existingState);
        
        // Reload to refresh properties
        SetupGame(existingState);
    }

    // Helper methods for view
    public ECellState GetCell(int x, int y) => Brain.GetBoard()[x, y];
    public int GetBoardWidth() => Brain.GetBoard().GetLength(0);
    public int GetBoardHeight() => Brain.GetBoard().GetLength(1);
    public bool IsWinningCell(int x, int y)
    {
        var cell = GetCell(x, y);
        return cell == ECellState.XWin || cell == ECellState.OWin;
    }
}