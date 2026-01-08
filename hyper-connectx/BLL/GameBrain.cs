namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    private string P1Name { get; set; }
    private string P2Name { get; set; }
    private EGameMode GameMode { get; set; }

    private bool NextMoveByX { get; set; } = true;

    public GameBrain(GameState gameState)
    {
        GameConfiguration = gameState.Configuration ?? new GameConfiguration();
        P1Name = gameState.P1Name;
        P2Name = gameState.P2Name;
        GameMode = gameState.GameMode;
        GameBoard = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
    }

    public string GetPlayerNames()
    {
        return P1Name + "(X) vs " + P2Name + "(O)";
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
    
    // NEW: Check if the board is full (draw condition)
    public bool IsBoardFull()
    {
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        {
            // If any column has an empty cell in the top row, board is not full
            if (GameBoard[x, 0] == ECellState.Empty)
            {
                return false;
            }
        }
        return true;
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
            var winningCells = new List<(int x, int y)> { (x, y) };
        
            // Check in positive direction
            var nextX = WrapX(x + dirX);
            var nextY = y + dirY;
            while (BoardCoordinatesAreValid(nextX, nextY) && 
                   GameBoard[x, y] == GameBoard[nextX, nextY] &&
                   count < GameConfiguration.ConnectHow)
            {
                count++;
                winningCells.Add((nextX, nextY));
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
                winningCells.Add((nextX, nextY));
                nextX = WrapX(nextX + flipDirX);
                nextY += flipDirY;
            }
        
            if (count >= GameConfiguration.ConnectHow)
            {
                // Mark all winning cells
                var winState = GameBoard[x, y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
                foreach (var (wx, wy) in winningCells)
                {
                    GameBoard[wx, wy] = winState;
                }
                return winState;
            }
        }

        return ECellState.Empty;
    }
    
    // Mark winner by checking all cells (used after a move to detect and mark wins)
    public ECellState? MarkWinner()
    {
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        {
            for (int y = 0; y < GameConfiguration.BoardHeight; y++)
            {
                if (GameBoard[x, y] == ECellState.X || GameBoard[x, y] == ECellState.O)
                {
                    var result = GetWinner(x, y);
                    if (result == ECellState.XWin || result == ECellState.OWin)
                    {
                        return result;
                    }
                }
            }
        }
        return null;
    }
    
    private static ECellState[][] ConvertToJagged(ECellState[,] board)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);
        var jagged = new ECellState[width][];

        for (int x = 0; x < width; x++)
        {
            jagged[x] = new ECellState[height];
            for (int y = 0; y < height; y++)
            {
                jagged[x][y] = board[x, y];
            }
        }

        return jagged;
    }

    private static ECellState[,] ConvertToRectangular(ECellState[][] jagged)
    {
        int width = jagged.Length;
        int height = jagged[0].Length;
        var rectangular = new ECellState[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rectangular[x, y] = jagged[x][y];
            }
        }

        return rectangular;
    }
    
    public GameState GetGameState()
    {
        return new GameState
        {
            Configuration = GameConfiguration,
            Board = ConvertToJagged(GameBoard),
            NextMoveByX = NextMoveByX,
            P1Name = P1Name,
            P2Name = P2Name,
            GameMode = GameMode
        };
    }

    public void LoadGameState(GameState state)
    {
        GameConfiguration = state.Configuration ?? new GameConfiguration();
        GameBoard = ConvertToRectangular(state.Board);
        NextMoveByX = state.NextMoveByX;
        P1Name = state.P1Name;
        P2Name = state.P2Name;
        GameMode = state.GameMode;
    }

    public bool GetIsCylindrical()
    {
        return GameConfiguration.IsCylindrical;
    }

    public EGameMode GetGameMode()
    {
        return GameMode;
    }
}