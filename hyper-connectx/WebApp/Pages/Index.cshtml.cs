using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<GameState> _gameRepository;

    public IndexModel(IRepository<GameState> gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public List<GameState> SavedGames { get; set; } = new();

    public void OnGet()
    {
        // Load all saved games using the repository's List method
        var gameList = _gameRepository.List();
        
        // Load each game state to get full details
        SavedGames = gameList
            .Select(g => _gameRepository.Load(g.id))
            .OrderByDescending(g => g.SaveName) // Order by save name (contains timestamp)
            .ToList();
    }

    public IActionResult OnPostDelete(string id)
    {
        _gameRepository.Delete(id);
        return RedirectToPage();
    }
}
