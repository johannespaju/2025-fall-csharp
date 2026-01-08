using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL;
using DAL;

namespace WebApp.Pages;

public class NewGameModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;
    private readonly IRepository<GameState> _gameRepository;

    public NewGameModel(
        IRepository<GameConfiguration> configRepository,
        IRepository<GameState> gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    // Saved configurations for dropdown
    public List<SelectListItem> SavedConfigurations { get; set; } = new();

    // Form binding properties
    [BindProperty]
    public string ConfigChoice { get; set; } = "new";

    [BindProperty]
    public Guid? SelectedConfigId { get; set; }

    [BindProperty]
    [Range(3, 20, ErrorMessage = "Board width must be between 3 and 20")]
    public int BoardWidth { get; set; } = 7;

    [BindProperty]
    [Range(3, 20, ErrorMessage = "Board height must be between 3 and 20")]
    public int BoardHeight { get; set; } = 6;

    [BindProperty]
    [Range(2, 20, ErrorMessage = "Connect How Many must be between 2 and the minimum of board dimensions")]
    public int ConnectHowMany { get; set; } = 4;

    [BindProperty]
    [Required(ErrorMessage = "Player 1 name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Player name must be 1-50 characters")]
    public string Player1Name { get; set; } = "Player 1";

    [BindProperty]
    [Required(ErrorMessage = "Player 2 name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Player name must be 1-50 characters")]
    public string Player2Name { get; set; } = "Player 2";

    [BindProperty]
    public EGameMode GameMode { get; set; } = EGameMode.PvP;

    [BindProperty]
    public bool IsCylindrical { get; set; } = false;

    // Validation error message
    public string? ValidationError { get; set; }

    public void OnGet()
    {
        LoadSavedConfigurations();
    }

    public IActionResult OnPost()
    {
        LoadSavedConfigurations();

        // If using existing config, load it
        GameConfiguration config;
        if (SelectedConfigId.HasValue && SelectedConfigId.Value != Guid.Empty)
        {
            config = _configRepository.Load(SelectedConfigId.Value.ToString())!;
            if (config == null)
            {
                ValidationError = "Selected configuration not found.";
                return Page();
            }
        }
        else
        {
            // Custom validation: ConnectHowMany must be <= min(BoardWidth, BoardHeight)
            var maxConnect = Math.Min(BoardWidth, BoardHeight);
            if (ConnectHowMany > maxConnect)
            {
                ValidationError = $"Connect How Many ({ConnectHowMany}) cannot exceed the minimum board dimension ({maxConnect})";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create new configuration
            config = new GameConfiguration
            {
                Id = Guid.NewGuid(),
                Name = $"Game Config {DateTime.Now:yyyy-MM-dd HH:mm}",
                BoardWidth = BoardWidth,
                BoardHeight = BoardHeight,
                ConnectHow = ConnectHowMany,
                Mode = GameMode,
                IsCylindrical = IsCylindrical
            };

            // Save the configuration
            _configRepository.Save(config);
        }

        // Create game brain with configuration
        // Create a temporary GameState to initialize GameBrain, then set player names
        var initialState = new GameState
        {
            Configuration = config,
            P1Name = Player1Name,
            P2Name = Player2Name
        };
        var brain = new GameBrain(initialState);
        
        // Get initial game state
        var gameState = brain.GetGameState();
        gameState.Id = Guid.NewGuid();
        gameState.GameConfigurationId = config.Id;
        gameState.SaveName = $"Game {DateTime.Now:yyyy-MM-dd HH:mm}";

        // Save the game state
        _gameRepository.Save(gameState);

        // Redirect to the game page
        return RedirectToPage("/Game", new { id = gameState.Id });
    }


    private void LoadSavedConfigurations()
    {
        var configs = _configRepository.List();
        
        SavedConfigurations = new List<SelectListItem>
        {
            new SelectListItem("-- Select a saved configuration --", "")
        };
        
        SavedConfigurations.AddRange(
            configs.Select(c => new SelectListItem(c.description, c.id))
        );
    }
}