namespace BLL;

public class GameConfiguration : BaseEntity
{
    public ICollection<GameState>?  GameStates { get; set; }
    
    public string Name { get; set; } = "Classical";
    
    public int BoardWidth { get; set; } = 7;
    public int BoardHeight { get; set; } = 6;
    
    public int ConnectHow { get; set; } = 4;
    
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
        Mode = EGameMode.PvP;
        IsCylindrical = false;
    }
    
    public void ApplyFrom(GameConfiguration other)
    {
        Name = other.Name;
        BoardWidth = other.BoardWidth;
        BoardHeight = other.BoardHeight;
        ConnectHow = other.ConnectHow;
        Mode = other.Mode;
        IsCylindrical = other.IsCylindrical;
    }
}
