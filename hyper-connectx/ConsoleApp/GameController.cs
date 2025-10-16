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
            Ui.GetPlayerNames(GameBrain.GetPlayerNames());
            Console.WriteLine();
            Ui.DrawBoard(GameBrain.GetBoard());
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

                var rowIndex = GameBrain.ProcessMove(columnIndex);

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
                    Ui.DrawBoard(GameBrain.GetBoard());
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

        } while (gameOver == false);
    }
}