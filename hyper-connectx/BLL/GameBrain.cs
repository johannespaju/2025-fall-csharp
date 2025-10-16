namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    public GameConfiguration GameConfiguration { get; set; }
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
        return P1Name + "(X) vs " + P2Name + " (O)";
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }
    
    public bool IsNextPlayerX() => NextMoveByX;

    // Calculate move without executing it - for validation and animation planning
    public MoveResult CalculateMove(int column)
    {
        // Find the lowest empty cell in column
        int targetRow = -1;
        for (int y = GameConfiguration.BoardHeight - 1; y >= 0; y--)
        {
            if (GameBoard[column, y] == ECellState.Empty)
            {
                targetRow = y;
                break;
            }
        }
    
        if (targetRow == -1)
        {
            return MoveResult.Invalid(); // Column is full
        }
    
        // Build the animation path (all rows from 0 to target)
        var animationPath = new List<int>();
        for (int y = 0; y <= targetRow; y++)
        {
            animationPath.Add(y);
        }
    
        var pieceToPlace = NextMoveByX ? ECellState.X : ECellState.O;
        
        return MoveResult.Valid(column, targetRow, animationPath, pieceToPlace);
    }

    // NEW: Execute a move (after validation/animation is done)
    public void ExecuteMove(int column, int row)
    {
        var pieceState = NextMoveByX ? ECellState.X : ECellState.O;
        GameBoard[column, row] = pieceState;
        NextMoveByX = !NextMoveByX;
    }
    
    private (int dirX, int dirY) GetDirection(int directionIndex) =>
        directionIndex switch
        {
            0 => (-1, -1),  // Diagonal up-left
            1 => (0, -1),   // Vertical
            2 => (1, -1),   // Diagonal up-right
            3 => (1, 0),    // horizontal
            _ => (0, 0)
        };
    
    private (int dirX, int dirY) FlipDirection((int dirX, int dirY) direction) =>
        (-direction.dirX, -direction.dirY);
    
    private bool BoardCoordinatesAreValid(int x, int y)
    {
        return x >= 0 && x < GameConfiguration.BoardWidth && 
               y >= 0 && y < GameConfiguration.BoardHeight;
    }
    
    private int WrapX(int x)
    {
        if (!GameConfiguration.IsCylindrical)
        {
            return x;
        }
    
        while (x < 0) x += GameConfiguration.BoardWidth;
        while (x >= GameConfiguration.BoardWidth) x -= GameConfiguration.BoardWidth;
        return x;
    }
    
    public ECellState GetWinner(int x, int y)
    {
        if (GameBoard[x, y] == ECellState.Empty) return ECellState.Empty;

        for (int directionIndex = 0; directionIndex < 4; directionIndex++)
        {
            var (dirX, dirY) = GetDirection(directionIndex);

            var count = 1; // Start at 1 to count the current cell
        
            // Check in positive direction
            var nextX = WrapX(x + dirX);
            var nextY = y + dirY;
            while (BoardCoordinatesAreValid(nextX, nextY) && 
                   GameBoard[x, y] == GameBoard[nextX, nextY] &&
                   count < GameConfiguration.ConnectHow)
            {
                count++;
                nextX = WrapX(nextX + dirX);
                nextY += dirY;
            }

            // Check in negative direction
            var (flipDirX, flipDirY) = FlipDirection((dirX, dirY));
            nextX = WrapX(x + flipDirX);
            nextY = y + flipDirY;
            while (BoardCoordinatesAreValid(nextX, nextY) && 
                   GameBoard[x, y] == GameBoard[nextX, nextY] &&
                   count < GameConfiguration.ConnectHow)
            {
                count++;
                nextX = WrapX(nextX + flipDirX);
                nextY += flipDirY;
            }
        
            if (count >= GameConfiguration.ConnectHow)
            {
                return GameBoard[x, y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
            }
        }

        return ECellState.Empty;
    }
}
