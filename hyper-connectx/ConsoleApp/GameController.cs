using BLL;
using ConsoleUI;

namespace ConsoleApp;

public class GameController
{
    private GameBrain GameBrain { get; set; }

    public GameController()
    {
        GameBrain = new GameBrain(new GameConfiguration(), "Player 1", "Player 2");
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

            Console.WriteLine("Choice x:");
            Console.WriteLine("Write x to exit");
            var input = Console.ReadLine();
            if (input?.ToLower() == "x")
            {
                gameOver = true;
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
                    Thread.Sleep(200);
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
            Thread.Sleep(100); // Adjust speed as needed

            // Clear the position for next frame (unless it's the final position)
            if (row < moveResult.FinalRow)
            {
                animatedBoard[moveResult.Column, row] = ECellState.Empty;
            }
        }
    }
}