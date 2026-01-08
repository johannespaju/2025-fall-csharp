namespace BLL;

public class MinimaxAI
{
    private readonly GameConfiguration _config;
    private readonly int _maxDepth;
    private readonly bool _isMaximizingPlayer; // true if AI is X, false if AI is O
    private readonly EAiDifficulty _difficulty;
    
    // Evaluation weights
    private const int WIN_SCORE = 100000;
    private const int THREE_IN_ROW = 100;
    private const int TWO_IN_ROW = 10;
    private const int CENTER_BONUS = 3;
    
    public MinimaxAI(GameConfiguration config, bool isPlayerX, EAiDifficulty difficulty = EAiDifficulty.Hard)
    {
        _config = config;
        _isMaximizingPlayer = isPlayerX;
        _difficulty = difficulty;
        _maxDepth = difficulty switch
        {
            EAiDifficulty.Easy => 0, // Will be handled as random move
            EAiDifficulty.Medium => 4,
            EAiDifficulty.Hard => 6,
            _ => 6
        };
    }
    
    public int GetBestMove(ECellState[,] board, bool nextMoveByX)
    {
        // Easy mode: random valid move
        if (_difficulty == EAiDifficulty.Easy)
        {
            return GetRandomMove(board);
        }
        
        int bestMove = -1;
        int bestScore = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        
        // Try each column
        for (int col = 0; col < _config.BoardWidth; col++)
        {
            int row = GetLowestEmptyRow(board, col);
            if (row == -1) continue; // Column full
            
            // Make move
            board[col, row] = nextMoveByX ? ECellState.X : ECellState.O;
            
            // Evaluate this move
            int score = Minimax(board, _maxDepth - 1, alpha, beta, false, !nextMoveByX);
            
            // Undo move
            board[col, row] = ECellState.Empty;
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = col;
            }
            
            alpha = Math.Max(alpha, bestScore);
        }
        
        // Fallback to center column if no move found
        return bestMove != -1 ? bestMove : _config.BoardWidth / 2;
    }
    
    private int GetRandomMove(ECellState[,] board)
    {
        var validMoves = new List<int>();
        for (int col = 0; col < _config.BoardWidth; col++)
        {
            if (GetLowestEmptyRow(board, col) != -1)
                validMoves.Add(col);
        }
        
        if (validMoves.Count == 0) return _config.BoardWidth / 2;
        
        var random = new Random();
        return validMoves[random.Next(validMoves.Count)];
    }
    
    private int Minimax(ECellState[,] board, int depth, int alpha, int beta, bool isMaximizing, bool nextMoveByX)
    {
        // Check for terminal states
        var winner = CheckWinner(board);
        if (winner != ECellState.Empty)
        {
            if ((winner == ECellState.XWin && _isMaximizingPlayer) || 
                (winner == ECellState.OWin && !_isMaximizingPlayer))
                return WIN_SCORE + depth; // Prefer faster wins
            else
                return -WIN_SCORE - depth; // Prefer slower losses
        }
        
        // Check for draw
        if (IsBoardFull(board))
            return 0;
        
        // Depth limit reached
        if (depth == 0)
            return EvaluateBoard(board);
        
        if (isMaximizing)
        {
            int maxScore = int.MinValue;
            
            for (int col = 0; col < _config.BoardWidth; col++)
            {
                int row = GetLowestEmptyRow(board, col);
                if (row == -1) continue;
                
                board[col, row] = nextMoveByX ? ECellState.X : ECellState.O;
                int score = Minimax(board, depth - 1, alpha, beta, false, !nextMoveByX);
                board[col, row] = ECellState.Empty;
                
                maxScore = Math.Max(maxScore, score);
                alpha = Math.Max(alpha, score);
                
                if (beta <= alpha)
                    break; // Beta cutoff
            }
            
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;
            
            for (int col = 0; col < _config.BoardWidth; col++)
            {
                int row = GetLowestEmptyRow(board, col);
                if (row == -1) continue;
                
                board[col, row] = nextMoveByX ? ECellState.X : ECellState.O;
                int score = Minimax(board, depth - 1, alpha, beta, true, !nextMoveByX);
                board[col, row] = ECellState.Empty;
                
                minScore = Math.Min(minScore, score);
                beta = Math.Min(beta, score);
                
                if (beta <= alpha)
                    break; // Alpha cutoff
            }
            
            return minScore;
        }
    }
    
    private int EvaluateBoard(ECellState[,] board)
    {
        int score = 0;
        
        // Evaluate all possible windows
        score += EvaluateWindows(board);
        
        // Center column preference
        int centerCol = _config.BoardWidth / 2;
        for (int row = 0; row < _config.BoardHeight; row++)
        {
            if (board[centerCol, row] == (_isMaximizingPlayer ? ECellState.X : ECellState.O))
                score += CENTER_BONUS;
            else if (board[centerCol, row] == (_isMaximizingPlayer ? ECellState.O : ECellState.X))
                score -= CENTER_BONUS;
        }
        
        return score;
    }
    
    private int EvaluateWindows(ECellState[,] board)
    {
        int score = 0;
        int connectLength = _config.ConnectHow;
        
        // Horizontal windows
        for (int row = 0; row < _config.BoardHeight; row++)
        {
            for (int col = 0; col <= _config.BoardWidth - connectLength; col++)
            {
                var window = new List<ECellState>();
                for (int i = 0; i < connectLength; i++)
                {
                    int checkCol = _config.IsCylindrical ? WrapX(col + i) : col + i;
                    window.Add(board[checkCol, row]);
                }
                score += EvaluateWindow(window);
            }
        }
        
        // Vertical windows
        for (int col = 0; col < _config.BoardWidth; col++)
        {
            for (int row = 0; row <= _config.BoardHeight - connectLength; row++)
            {
                var window = new List<ECellState>();
                for (int i = 0; i < connectLength; i++)
                    window.Add(board[col, row + i]);
                score += EvaluateWindow(window);
            }
        }
        
        // Diagonal (down-right)
        for (int row = 0; row <= _config.BoardHeight - connectLength; row++)
        {
            for (int col = 0; col <= _config.BoardWidth - connectLength; col++)
            {
                var window = new List<ECellState>();
                for (int i = 0; i < connectLength; i++)
                {
                    int checkCol = _config.IsCylindrical ? WrapX(col + i) : col + i;
                    window.Add(board[checkCol, row + i]);
                }
                score += EvaluateWindow(window);
            }
        }
        
        // Diagonal (up-right)
        for (int row = connectLength - 1; row < _config.BoardHeight; row++)
        {
            for (int col = 0; col <= _config.BoardWidth - connectLength; col++)
            {
                var window = new List<ECellState>();
                for (int i = 0; i < connectLength; i++)
                {
                    int checkCol = _config.IsCylindrical ? WrapX(col + i) : col + i;
                    window.Add(board[checkCol, row - i]);
                }
                score += EvaluateWindow(window);
            }
        }
        
        return score;
    }
    
    private int EvaluateWindow(List<ECellState> window)
    {
        int aiPieces = 0;
        int opponentPieces = 0;
        int empty = 0;
        
        ECellState aiPiece = _isMaximizingPlayer ? ECellState.X : ECellState.O;
        ECellState opponentPiece = _isMaximizingPlayer ? ECellState.O : ECellState.X;
        
        foreach (var cell in window)
        {
            if (cell == aiPiece) aiPieces++;
            else if (cell == opponentPiece) opponentPieces++;
            else empty++;
        }
        
        // If window has both players, it's not useful
        if (aiPieces > 0 && opponentPieces > 0)
            return 0;
        
        // Score based on how many pieces
        if (aiPieces == _config.ConnectHow - 1 && empty == 1)
            return THREE_IN_ROW; // One move from winning
        if (aiPieces == _config.ConnectHow - 2 && empty == 2)
            return TWO_IN_ROW;
        
        if (opponentPieces == _config.ConnectHow - 1 && empty == 1)
            return -THREE_IN_ROW * 2; // Block opponent's win (higher priority)
        if (opponentPieces == _config.ConnectHow - 2 && empty == 2)
            return -TWO_IN_ROW;
        
        return 0;
    }
    
    private int GetLowestEmptyRow(ECellState[,] board, int col)
    {
        for (int row = _config.BoardHeight - 1; row >= 0; row--)
        {
            if (board[col, row] == ECellState.Empty)
                return row;
        }
        return -1; // Column full
    }
    
    private ECellState CheckWinner(ECellState[,] board)
    {
        // Check all positions for a win
        for (int x = 0; x < _config.BoardWidth; x++)
        {
            for (int y = 0; y < _config.BoardHeight; y++)
            {
                if (board[x, y] == ECellState.Empty) continue;
                
                var winner = CheckWinFromPosition(board, x, y);
                if (winner != ECellState.Empty)
                    return winner;
            }
        }
        return ECellState.Empty;
    }
    
    private ECellState CheckWinFromPosition(ECellState[,] board, int x, int y)
    {
        if (board[x, y] == ECellState.Empty) return ECellState.Empty;
        
        // Check all 4 directions
        for (int directionIndex = 0; directionIndex < 4; directionIndex++)
        {
            var (dirX, dirY) = GetDirection(directionIndex);
            int count = 1;
            
            // Positive direction
            int nextX = WrapX(x + dirX);
            int nextY = y + dirY;
            while (IsValidPosition(nextX, nextY) && 
                   board[x, y] == board[nextX, nextY] &&
                   count < _config.ConnectHow)
            {
                count++;
                nextX = WrapX(nextX + dirX);
                nextY += dirY;
            }
            
            // Negative direction
            nextX = WrapX(x - dirX);
            nextY = y - dirY;
            while (IsValidPosition(nextX, nextY) && 
                   board[x, y] == board[nextX, nextY] &&
                   count < _config.ConnectHow)
            {
                count++;
                nextX = WrapX(nextX - dirX);
                nextY -= dirY;
            }
            
            if (count >= _config.ConnectHow)
                return board[x, y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
        }
        
        return ECellState.Empty;
    }
    
    private bool IsBoardFull(ECellState[,] board)
    {
        for (int col = 0; col < _config.BoardWidth; col++)
        {
            if (board[col, 0] == ECellState.Empty)
                return false;
        }
        return true;
    }
    
    private (int dirX, int dirY) GetDirection(int directionIndex) =>
        directionIndex switch
        {
            0 => (-1, -1),  // Diagonal up-left
            1 => (0, -1),   // Vertical
            2 => (1, -1),   // Diagonal up-right
            3 => (1, 0),    // Horizontal
            _ => (0, 0)
        };
    
    private int WrapX(int x)
    {
        if (!_config.IsCylindrical) return x;
        
        while (x < 0) x += _config.BoardWidth;
        while (x >= _config.BoardWidth) x -= _config.BoardWidth;
        return x;
    }
    
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < _config.BoardWidth && 
               y >= 0 && y < _config.BoardHeight;
    }
}