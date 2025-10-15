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

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }
    
    public bool IsNextPlayerX() => NextMoveByX;

    public void ProcessMove(int x, int y)
    {
        // TODO: validate input
        
        // TODO: Make it follow ConnectX not tic-tac-toe logic
        if (GameBoard[x, y] == ECellState.Empty)
        {
            GameBoard[x, y] = NextMoveByX ? ECellState.X :  ECellState.O;
            NextMoveByX = !NextMoveByX;
        } 
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
    
    public bool BoardCoordinatesAreValid(int x, int y)
    {
        if (x < 0 || x >= (GameConfiguration.BoardWidth - 1)) return false;
        if (y < 0 || y >= (GameConfiguration.BoardHeight - 1)) return false;
        return true;
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
            while (BoardCoordinatesAreValid(nextX, nextY) && GameBoard[x, y] == GameBoard[nextX, nextY] &&
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
                while (BoardCoordinatesAreValid(nextX, nextY) && GameBoard[x, y] == GameBoard[nextX, nextY] &&
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
