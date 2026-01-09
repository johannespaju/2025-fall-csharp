using System.Text.RegularExpressions;
using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryEF : IRepository<GameState>
{
    private readonly AppDbContext _dbContext;
    
    public GameRepositoryEF(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<(string id, string description, bool isHidden)> List()
    {
        var res = new List<(string id, string description, bool isHidden)>();
        foreach (var dbGame in _dbContext.GameStates.Include(g => g.Configuration).ToList()) // Add .ToList()
        {
            var description = dbGame.SaveName;
            if (dbGame.Configuration != null)
            {
                description += $" - {dbGame.Configuration.BoardWidth}x{dbGame.Configuration.BoardHeight}";
            }
        
            res.Add((dbGame.Id.ToString(), description, false));
        }
    
        return res;
    }
    
    public async Task<List<(string id, string description, bool isHidden)>> ListAsync()
    {
        var res = new List<(string id, string description, bool isHidden)>();
        foreach (var dbGame in await _dbContext.GameStates.Include(gameState => gameState.Configuration).ToListAsync())
        {
            var description = dbGame.SaveName;
            if (dbGame.Configuration != null)
            {
                description += $" - {dbGame.Configuration.BoardWidth}x{dbGame.Configuration.BoardHeight}";
            }

            res.Add(
                (
                    dbGame.Id.ToString(),
                    description,
                    false
                )
            );
        }

        return res;
    }

    public string Save(GameState data)
    {
        var safeName = Regex.Replace(data.SaveName.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        data.SaveName = safeName;

        // Check if this is an update by ID (not by SaveName) to avoid foreign key issues
        var existing = _dbContext.GameStates
            .FirstOrDefault(g => g.Id == data.Id);

        if (existing == null)
        {
            // new game state - also check if SaveName already exists to avoid duplicate names
            var existingByName = _dbContext.GameStates
                .FirstOrDefault(g => g.SaveName == safeName);
            
            if (existingByName != null)
            {
                // Generate a unique name
                data.SaveName = $"{safeName}_{Guid.NewGuid():N}";
            }
            
            _dbContext.GameStates.Add(data);
            _dbContext.SaveChanges();
            return data.Id.ToString();
        }
        else
        {
            // update existing game state - manually copy properties except Id
            existing.SaveName = data.SaveName;
            existing.GameConfigurationId = data.GameConfigurationId;
            existing.BoardJson = data.BoardJson;
            existing.NextMoveByX = data.NextMoveByX;
            existing.LastMoveColumn = data.LastMoveColumn;
            existing.LastMoveRow = data.LastMoveRow;
            existing.Status = data.Status;
            existing.P1Name = data.P1Name;
            existing.P2Name = data.P2Name;
            existing.GameMode = data.GameMode;
            existing.Difficulty = data.Difficulty;

            _dbContext.SaveChanges();
            return existing.Id.ToString();
        }
    }

    public GameState Load(string id)
    {
        Console.WriteLine($"DEBUG: Trying to load game ID: '{id}'");
    
        if (!Guid.TryParse(id, out var guidId))
        {
            throw new ArgumentException($"Invalid ID format: '{id}'");
        }
    
        var game = _dbContext.GameStates
            .Include(g => g.Configuration)
            .FirstOrDefault(g => g.Id == guidId);

        if (game == null)
        {
            throw new KeyNotFoundException($"Game state '{id}' not found.");
        }

        return game;
    }

    public void Delete(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
        {
            return; // Invalid ID, nothing to delete
        }
    
        var game = _dbContext.GameStates
            .FirstOrDefault(g => g.Id == guidId);

        if (game == null)
        {
            return;
        }

        _dbContext.GameStates.Remove(game);
        _dbContext.SaveChanges();
    }
    // Async methods
    public async Task<string> SaveAsync(GameState data)
    {
        var safeName = Regex.Replace(data.SaveName.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        data.SaveName = safeName;

        // Check if this is an update by ID (not by SaveName) to avoid foreign key issues
        var existing = await _dbContext.GameStates
            .FirstOrDefaultAsync(g => g.Id == data.Id);

        if (existing == null)
        {
            // new game state - also check if SaveName already exists to avoid duplicate names
            var existingByName = await _dbContext.GameStates
                .FirstOrDefaultAsync(g => g.SaveName == safeName);
            
            if (existingByName != null)
            {
                // Generate a unique name
                data.SaveName = $"{safeName}_{Guid.NewGuid():N}";
            }
            
            await _dbContext.GameStates.AddAsync(data);
            await _dbContext.SaveChangesAsync();
            return data.Id.ToString();
        }
        else
        {
            existing.SaveName = data.SaveName;
            existing.GameConfigurationId = data.GameConfigurationId;
            existing.BoardJson = data.BoardJson;
            existing.NextMoveByX = data.NextMoveByX;
            existing.LastMoveColumn = data.LastMoveColumn;
            existing.LastMoveRow = data.LastMoveRow;
            existing.Status = data.Status;
            existing.P1Name = data.P1Name;
            existing.P2Name = data.P2Name;
            existing.GameMode = data.GameMode;
            existing.Difficulty = data.Difficulty;
            await _dbContext.SaveChangesAsync();
            return existing.Id.ToString();
        }
    }

    public async Task<GameState> LoadAsync(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            throw new ArgumentException($"Invalid ID format: '{id}'");

        var game = await _dbContext.GameStates
            .Include(g => g.Configuration)
            .FirstOrDefaultAsync(g => g.Id == guidId);

        if (game == null)
            throw new KeyNotFoundException($"Game state '{id}' not found.");

        return game;
    }

    public async Task DeleteAsync(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return;

        var game = await _dbContext.GameStates
            .FirstOrDefaultAsync(g => g.Id == guidId);

        if (game == null)
            return;

        _dbContext.GameStates.Remove(game);
        await _dbContext.SaveChangesAsync();
    }
}
