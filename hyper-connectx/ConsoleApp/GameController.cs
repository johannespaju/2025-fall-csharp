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
        int selectedIndex = 0;
        var gameOver = false;
        do
        {
            Console.Clear();

            // Draw the board
            Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(GameBrain.GetBoard(), GameBrain.GameConfiguration.IsCylindrical, selectedIndex);
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            Console.WriteLine("Press escape to leave game");
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
                    if (selectedIndex > GameBrain.GetBoard().GetLength(0))
                    {
                        selectedIndex = 0;
                    }
                    break;
                case ConsoleKey.Escape:
                    gameOver = true;
                    break;
                
                case ConsoleKey.S:
                    var repo = new GameRepositoryJson();
                var state = GameBrain.GetGameState();

                Console.Write("Enter a name for this save: ");
                var name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(name))
                    state.SaveName = name + "_" + DateTime.UtcNow.ToString("MM-dd_hh_mm");

                repo.Save(state);
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
                        gameOver = true;
                    }
                    break;
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