using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BLL;

public class GameState : BaseEntity
{
    public Guid GameConfigurationId { get; set; }
    public GameConfiguration? Configuration { get; set; } = new();
    
    public string BoardJson { get; set; } = default!;
    
    [NotMapped]
    public ECellState[][] Board
    {
        get => JsonSerializer.Deserialize<ECellState[][]>(BoardJson) ?? Array.Empty<ECellState[]>();
        set => BoardJson = JsonSerializer.Serialize(value);
    }
    public bool NextMoveByX { get; set; }
    public string SaveName { get; set; } = $"Game_{DateTime.UtcNow:yyyy-MM-dd_HH-mm}";
    
    public string P1Name { get; set; } = "Player 1";
    public string P2Name { get; set; } = "Player 2";
    
    public EGameMode GameMode { get; set; } = EGameMode.PvP;
}