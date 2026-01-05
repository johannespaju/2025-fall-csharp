using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages;

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

    public void OnGet()
    {
        // Load all saved games using the repository's List method
        var gameList = _gameRepo.List();
        
        // Load each game state to get full details
        SavedGames = gameList
            .Select(g => _gameRepo.Load(g.id))
            .OrderByDescending(g => g.SaveName) // Order by save name (contains timestamp)
            .ToList();
    }

    public IActionResult OnPostDelete(string id)
    {
        _gameRepo.Delete(id);
        return RedirectToPage();
    }
}