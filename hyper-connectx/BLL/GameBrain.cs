namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    private string P1Name { get; set; }
    private string P2Name { get; set; }

    private bool NextMoveByX { get; set; } = true;

    public GameBrain(GameConfiguration configuration, string p1Name, string p2Name)
    {
        GameConfiguration = configuration;
        P1Name = p1Name;
        P2Name = p2Name;
        GameBoard = new ECellState[configuration.BoardWidth, configuration.BoardHeight];
    }

    public string GetPlayerNames()
    {
        return P1Name + "(x) vs " + P2Name + " (o)";
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }
    
    public bool IsNextPlayerX() => NextMoveByX;

    public int ProcessMove(int x)
    {
        // Find the lowest empty cell in column x
        int targetY = -1;
        for (int y = GameConfiguration.BoardHeight - 1; y >= 0; y--)
        {
            if (GameBoard[x, y] == ECellState.Empty)
            {
                targetY = y;
                break;
            }
        }
    
        if (targetY == -1) return -1; // Column is full
    
        // Place the piece at the target position
        GameBoard[x, targetY] = NextMoveByX ? ECellState.X : ECellState.O;
        NextMoveByX = !NextMoveByX;
        return targetY;
    }

// Add a new method for animated move
    public int ProcessMoveWithAnimation(int x, Action<ECellState[,], int, int> drawCallback)
    {
        // Find the lowest empty cell in column x
        int targetY = -1;
        for (int y = GameConfiguration.BoardHeight - 1; y >= 0; y--)
        {
            if (GameBoard[x, y] == ECellState.Empty)
            {
                targetY = y;
                break;
            }
        }
    
        if (targetY == -1) return -1; // Column is full
    
        var pieceState = NextMoveByX ? ECellState.X : ECellState.O;
    
        // Animate the piece falling
        for (int y = 0; y <= targetY; y++)
        {
            // Temporarily place piece at current position
            GameBoard[x, y] = pieceState;
        
            // Callback to redraw the board
            drawCallback(GetBoard(), x, y);
        
            // Small delay for animation
            Thread.Sleep(50); // Adjust speed as needed
        
            // Clear the temporary position (unless it's the final position)
            if (y < targetY)
            {
                GameBoard[x, y] = ECellState.Empty;
            }
        }
    
        NextMoveByX = !NextMoveByX;
        return targetY;
    }
    
    
    private (int dirX, int dirY) GetDirection(int directionIndex) =>
        directionIndex switch
        {
            0 => (-1, -1),  // Diagonal up-left
            1 => (0, -1),  // Vertical
            2 => (1, -1),  // Diagonal up-right
            3 => (1, 0), // horizontal
            _ => (0, 0)
        };
    
    
    private (int dirX, int dirY) FlipDirection((int dirX, int dirY) direction) =>
        (-direction.dirX, -direction.dirY);
    
    private bool BoardCoordinatesAreValid(int x, int y)
    {
        return x >= 0 && x < GameConfiguration.BoardWidth && 
               y >= 0 && y < GameConfiguration.BoardHeight;
    }
    
    public ECellState GetWinner(int x, int y)
    {
        if (GameBoard[x, y] == ECellState.Empty) return ECellState.Empty;

        
        for (int directionIndex = 0; directionIndex < 4; directionIndex++)
        {
            var (dirX, dirY) = GetDirection(directionIndex);

            var count = 0;
            
            var nextX = x;
            var nextY = y;
            while (BoardCoordinatesAreValid(nextX, nextY) && 
                   GameBoard[x, y] == GameBoard[nextX, nextY] &&
                   count <= GameConfiguration.ConnectHow)
            {
                count++;
                nextX += dirX;
                nextY += dirY;
            }

            if (count < GameConfiguration.ConnectHow)
            {
                (dirX, dirY) = FlipDirection((dirX, dirY));
                nextX = x + dirX;
                nextY = y + dirY;
                while (BoardCoordinatesAreValid(nextX, nextY) && 
                       GameBoard[x, y] == GameBoard[nextX, nextY] &&
                       count <= GameConfiguration.ConnectHow)
                {
                    count++;
                    nextX += dirX;
                    nextY += dirY;
                }
            }
            
            if (count == GameConfiguration.ConnectHow)
            {
                return GameBoard[x, y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
            }

        }


        return ECellState.Empty;
    }

}
