using BLL;
using ConsoleUI;
using DAL;

namespace ConsoleApp;

public class GameController
{
    private IRepository<GameState> Repo {get; set;}
    internal GameBrain GameBrain { get; set; }
    private MinimaxAI? _aiPlayerX;  // AI for X player
    private MinimaxAI? _aiPlayerO;  // AI for O player
    private GameConfiguration _config;

    public GameController(GameConfiguration configuration, IRepository<GameState> gameRepository)
    {
        GameBrain = new GameBrain(configuration);
        Repo = gameRepository;
        _config = configuration;
        
        // Initialize AI if needed
        if (configuration.Mode == EGameMode.PvC)
        {
            // AI plays as O (second player)
            _aiPlayerO = new MinimaxAI(configuration, isPlayerX: false, maxDepth: 6);
        }
        else if (configuration.Mode == EGameMode.CvC)
        {
            // For CvC, create both AIs
            _aiPlayerX = new MinimaxAI(configuration, isPlayerX: true, maxDepth: 6);
            _aiPlayerO = new MinimaxAI(configuration, isPlayerX: false, maxDepth: 6);
        }
    }

    public void GameLoop()
    {
        int selectedIndex = 0;
        var gameOver = false;
        do
        {
            // Check if it's AI's turn
            bool isAiTurn = ShouldAiMove();
            
            if (isAiTurn)
            {
                // AI makes a move
                Thread.Sleep(500); // Small delay so user can see board
                MakeAiMove();
                
                // Check for winner after AI move
                var lastMove = GetLastMove();
                if (lastMove.col != -1)
                {
                    var winner = GameBrain.GetWinner(lastMove.col, lastMove.row);
                    if (winner != ECellState.Empty)
                    {
                        Console.Clear();
                        Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
                        Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? GameBrain.GetPlayerNames().Split(" vs ")[0] : GameBrain.GetPlayerNames().Split(" vs ")[1]));
                        Console.WriteLine("Press any key to continue...");
                        Thread.Sleep(150);
                        Console.ReadKey();
                        gameOver = true;
                    }
                    // Check for draw after AI move
                    else if (GameBrain.IsBoardFull())
                    {
                        Console.Clear();
                        Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
                        Console.WriteLine("It's a DRAW! The board is full with no winner.");
                        Console.WriteLine("Press any key to continue...");
                        Thread.Sleep(150);
                        Console.ReadKey();
                        gameOver = true;
                    }
                }
                continue; // Skip to next iteration
            }
            
            Console.Clear();

            // Draw the board
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX(), selectedIndex);
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            Console.WriteLine("Press \"escape\" or \"q\" to leave game");
            Console.WriteLine("Press \"s\" to save game");
            var keyPressed = Console.ReadKey(true).Key;
            switch (keyPressed)
            {
                case ConsoleKey.LeftArrow:
                    selectedIndex--;
                    if (selectedIndex < 0)
                    {
                        selectedIndex = GameBrain.GetBoard().GetLength(0) - 1;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    selectedIndex++;
                    if (selectedIndex > GameBrain.GetBoard().GetLength(0) - 1)
                    {
                        selectedIndex = 0;
                    }
                    break;
                case ConsoleKey.Escape or ConsoleKey.Q:
                    gameOver = true;
                    break;
                
                case ConsoleKey.S:
                    var state = GameBrain.GetGameState();

                    Console.Write("Enter a name for this save: ");
                    var name = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                        state.SaveName = name + "_" + DateTime.Now.ToString("MM-dd_hh_mm");

                    Repo.Save(state);
                    Console.WriteLine($"Game saved as '{state.SaveName}'.");
                    Thread.Sleep(1000);
                    continue;

                
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    // Calculate the move (without executing it yet)
                    var moveResult = GameBrain.CalculateMove(selectedIndex);

                    // Check if column was full
                    if (!moveResult.IsValid)
                    {
                        Console.WriteLine("Column is full! Press any key to continue...");
                        Console.ReadKey();
                        continue;
                    }
                    
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    
                    AnimatePieceFalling(moveResult);

                    // Execute the move in the game brain
                    GameBrain.ExecuteMove(moveResult.Column, moveResult.FinalRow);

                    // Check for winner
                    var winner = GameBrain.GetWinner(moveResult.Column, moveResult.FinalRow);
                    if (winner != ECellState.Empty)
                    {
                        Console.Clear();
                        Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
                        Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? GameBrain.GetPlayerNames().Split(" vs ")[0] : GameBrain.GetPlayerNames().Split(" vs ")[1]));
                        Console.WriteLine("Press any key to continue...");
                        Thread.Sleep(150);
                        Console.ReadKey();
                        gameOver = true;
                    }
                    // Check for draw
                    else if (GameBrain.IsBoardFull())
                    {
                        Console.Clear();
                        Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
                        Console.WriteLine("It's a DRAW! The board is full with no winner.");
                        Console.WriteLine("Press any key to continue...");
                        Thread.Sleep(150);
                        Console.ReadKey();
                        gameOver = true;
                    }
                    break;
            }
        } while (!gameOver);
    }

    private bool ShouldAiMove()
    {
        if (_config.Mode == EGameMode.PvP)
            return false;
        
        if (_config.Mode == EGameMode.PvC)
        {
            // AI is O (second player), so moves when NextMoveByX is false
            return !GameBrain.IsNextPlayerX();
        }
        
        if (_config.Mode == EGameMode.CvC)
        {
            // Both players are AI
            return true;
        }
        
        return false;
    }

    private void MakeAiMove()
    {
        // Select the correct AI based on whose turn it is
        MinimaxAI? currentAI = GameBrain.IsNextPlayerX() ? _aiPlayerX : _aiPlayerO;
        
        if (currentAI == null) return;
        
        var board = GameBrain.GetBoard();
        int bestColumn = currentAI.GetBestMove(board, GameBrain.IsNextPlayerX());
        
        // Calculate and execute the move
        var moveResult = GameBrain.CalculateMove(bestColumn);
        if (!moveResult.IsValid)
        {
            // Fallback: find first valid column
            for (int col = 0; col < _config.BoardWidth; col++)
            {
                moveResult = GameBrain.CalculateMove(col);
                if (moveResult.IsValid) break;
            }
        }
        
        if (moveResult.IsValid)
        {
            Console.Clear();
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());
            Console.WriteLine("\nAI is thinking...");
            
            AnimatePieceFalling(moveResult);
            GameBrain.ExecuteMove(moveResult.Column, moveResult.FinalRow);
        }
    }

    private (int col, int row) GetLastMove()
    {
        // Find the last placed piece by scanning the board
        var board = GameBrain.GetBoard();
        for (int col = 0; col < _config.BoardWidth; col++)
        {
            for (int row = 0; row < _config.BoardHeight; row++)
            {
                if (board[col, row] != ECellState.Empty)
                {
                    // This is a heuristic - in a real implementation you'd track this
                    return (col, row);
                }
            }
        }
        return (-1, -1);
    }

    // UI layer handles animation using the move result data
    private void AnimatePieceFalling(MoveResult moveResult)
    {
        // Create a temporary board state for animation
        var animatedBoard = GameBrain.GetBoard();

        foreach (var row in moveResult.AnimationPath)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true); // Consume and discard the key
            }
            
            // Place piece at current animation position
            animatedBoard[moveResult.Column, row] = moveResult.PiecePlaced;

            // Redraw the board
            Console.Clear();
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(animatedBoard, GameBrain.GetIsCylindrical(), GameBrain.IsNextPlayerX());
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            // Animation delay
            Thread.Sleep(100); // Adjust speed as needed

            // Clear the position for next frame (unless it's the final position)
            if (row < moveResult.FinalRow)
            {
                animatedBoard[moveResult.Column, row] = ECellState.Empty;
            }
            
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }
    }
}