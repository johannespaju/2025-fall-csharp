namespace BLL;

public class GameConfiguration : BaseEntity
{
    public ICollection<GameState>?  GameStates { get; set; }
    
    public string Name { get; set; } = "Classical";
    
    public int BoardWidth { get; set; } = 7;
    public int BoardHeight { get; set; } = 6;
    
    public int ConnectHow { get; set; } = 4;
    
    public string P1Name { get; set; } = "Player 1";
    public string P2Name { get; set; } = "Player 2";
    
    // Game mode (PvP, PvAi, AivAi)
    public EGameMode Mode { get; set; } = EGameMode.PvP;
    
    public bool IsCylindrical { get; set; } = false;
    
    // Reset to default settings
    public void ResetToDefault()
    {
        Name = "Classical";
        BoardWidth = 7;
        BoardHeight = 6;
        ConnectHow = 4;
        P1Name = "Player 1";
        P2Name = "Player 2";
        Mode = EGameMode.PvP;
        IsCylindrical = false;
    }
    
    public void ApplyFrom(GameConfiguration other)
    {
        Name = other.Name;
        BoardWidth = other.BoardWidth;
        BoardHeight = other.BoardHeight;
        ConnectHow = other.ConnectHow;
        P1Name = other.P1Name;
        P2Name = other.P2Name;
        Mode = other.Mode;
        IsCylindrical = other.IsCylindrical;
    }
}
