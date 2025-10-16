namespace BLL;

public class MoveResult
{
    public bool IsValid { get; set; }
    public int Column { get; set; }
    public int FinalRow { get; set; }
    public List<int> AnimationPath { get; set; } = new();
    public ECellState PiecePlaced { get; set; }
    
    // Factory methods for clarity
    public static MoveResult Invalid()
    {
        return new MoveResult { IsValid = false, FinalRow = -1 };
    }
    
    public static MoveResult Valid(int column, int finalRow, List<int> path, ECellState piece)
    {
        return new MoveResult
        {
            IsValid = true,
            Column = column,
            FinalRow = finalRow,
            AnimationPath = path,
            PiecePlaced = piece
        };
    }
}