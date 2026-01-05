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
    
    public List<(string id, string description)> List()
    {
        var res = new List<(string id, string description)>();
        foreach (var dbGame in _dbContext.GameStates.Include(g => g.Configuration).ToList()) // Add .ToList()
        {
            var description = dbGame.SaveName;
            if (dbGame.Configuration != null)
            {
                description += $" - {dbGame.Configuration.BoardWidth}x{dbGame.Configuration.BoardHeight}";
            }
        
            res.Add((dbGame.Id.ToString(), description));
        }
    
        return res;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        var res = new List<(string id, string description)>();
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
                    description
                )
            );
        }

        return res;
    }

    public string Save(GameState data)
    {
        var safeName = Regex.Replace(data.SaveName.Trim(), @"[^a-zA-Z0-9 _\-]", "_");
        data.SaveName = safeName;
    
        var existing = _dbContext.GameStates
            .FirstOrDefault(g => g.SaveName == safeName);
    
        if (existing == null)
        {
            // new game state
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

        var existing = await _dbContext.GameStates
            .FirstOrDefaultAsync(g => g.SaveName == safeName);

        if (existing == null)
        {
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