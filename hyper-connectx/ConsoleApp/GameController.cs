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

            // draw the board
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

                var rowIndex = GameBrain.ProcessMoveWithAnimation(columnIndex, (board, x, y) =>
                {
                    Console.Clear();
                    Ui.ShowPlayerNames(GameBrain.GetPlayerNames());
                    Console.WriteLine();
                    Ui.DrawBoard(board, GameBrain.GameConfiguration.IsCylindrical); // Use the passed board
                    Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());
                    // Optionally highlight the falling piece position at (x, y)
                });

                // Check if column was full
                if (rowIndex == -1)
                {
                    Console.WriteLine("Column is full! Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                var winner = GameBrain.GetWinner(columnIndex, rowIndex);
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
}