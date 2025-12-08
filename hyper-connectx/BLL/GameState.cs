namespace BLL;

public class GameState : BaseEntity
{
    public Guid GameConfigurationId { get; set; }
    public GameConfiguration Configuration { get; set; } = new();
    
    public ECellState[][] Board { get; set; } = default!;
    public bool NextMoveByX { get; set; }
    public string SaveName { get; set; } = $"Save_{DateTime.UtcNow:MM-dd_hh_mm}";
}