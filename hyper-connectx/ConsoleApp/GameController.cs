using BLL;
using ConsoleUI;
using DLL;

namespace ConsoleApp;

public class GameController
{
    internal GameBrain GameBrain { get; set; }

    public GameController(GameConfiguration configuration)
    {
        GameBrain = new GameBrain(configuration, configuration.P1Name, configuration.P2Name);
    }

    public void GameLoop()
    {
        var gameOver = false;
        do
        {
            Console.Clear();
            
            // Draw the board
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GameConfiguration.IsCylindrical);
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            Console.WriteLine("Write x to exit");
            Console.WriteLine("Write 'save' to save game");
            Console.WriteLine("Choice x:");
            var input = Console.ReadLine();
            if (input?.ToLower() == "x")
            {
                gameOver = true;
                continue;
            }
            if (input?.ToLower() == "save")
            {
                var repo = new GameRepositoryJson();
                var state = GameBrain.GetGameState();

                Console.Write("Enter a name for this save: ");
                var name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(name))
                    state.SaveName = name;

                repo.Save(state);
                Console.WriteLine($"Game saved as '{state.SaveName}'.");
                Thread.Sleep(1000);
                continue;
            }

            // Validate input
            if (input == null) continue;

            if (int.TryParse(input, out var x))
            {
                // Adjust for 1-based user input to 0-based array index
                var columnIndex = x - 1;

                // Validate column is within bounds
                if (columnIndex < 0 || columnIndex >= GameBrain.GetBoard().GetLength(0))
                {
                    Console.WriteLine("Invalid column! Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                // Calculate the move (without executing it yet)
                var moveResult = GameBrain.CalculateMove(columnIndex);

                // Check if column was full
                if (!moveResult.IsValid)
                {
                    Console.WriteLine("Column is full! Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                // Animate the piece falling (UI layer responsibility)
                AnimatePieceFalling(moveResult);

                // Execute the move in the game brain
                GameBrain.ExecuteMove(moveResult.Column, moveResult.FinalRow);

                // Check for winner
                var winner = GameBrain.GetWinner(moveResult.Column, moveResult.FinalRow);
                if (winner != ECellState.Empty)
                {
                    Console.Clear();
                    Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GameConfiguration.IsCylindrical);
                    Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? "X" : "O"));
                    Console.WriteLine("Press any key to continue...");
                    Thread.Sleep(150);
                    Console.ReadKey();
                    break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input! Press any key to continue...");
                Console.ReadKey();
            }

        } while (!gameOver);
    }

    // UI layer handles animation using the move result data
    private void AnimatePieceFalling(MoveResult moveResult)
    {
        // Create a temporary board state for animation
        var animatedBoard = GameBrain.GetBoard();

        foreach (var row in moveResult.AnimationPath)
        {
            // Place piece at current animation position
            animatedBoard[moveResult.Column, row] = moveResult.PiecePlaced;

            // Redraw the board
            Console.Clear();
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(animatedBoard, GameBrain.GameConfiguration.IsCylindrical);
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            // Animation delay
            Thread.Sleep(200); // Adjust speed as needed

            // Clear the position for next frame (unless it's the final position)
            if (row < moveResult.FinalRow)
            {
                animatedBoard[moveResult.Column, row] = ECellState.Empty;
            }
        }
    }
}