using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    private readonly IRepository<GameState> _gameRepo;
    
    public IndexModel(IRepository<GameConfiguration> configRepo, IRepository<GameState> gameRepo)
    {
        _configRepo = configRepo;
        _gameRepo = gameRepo;
    }
    
    public List<GameState> SavedGames { get; set; } = new();
    

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        Console.WriteLine($"DEBUG: OnPostDeleteAsync called with id: '{id}'");
        await _gameRepo.DeleteAsync(id);
        return RedirectToPage();
    }
    
    public async Task OnGetAsync()
    {
        // Load all saved games using the repository's async List method
        var gameList = await _gameRepo.ListAsync();
    
        // Load each game state sequentially
        var loadedGames = new List<GameState>(); // Replace with your actual type
        foreach (var game in gameList)
        {
            var loadedGame = await _gameRepo.LoadAsync(game.id);
            loadedGames.Add(loadedGame);
        }
    
        // Order by save name
        SavedGames = loadedGames
            .OrderByDescending(g => g.SaveName)
            .ToList();
    }
}