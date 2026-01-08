using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages;

public class ConfigManagerModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;
    private readonly IRepository<GameState> _gameRepository;

    public ConfigManagerModel(
        IRepository<GameConfiguration> configRepository,
        IRepository<GameState> gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    // List of saved configurations
    public List<GameConfiguration> SavedConfigurations { get; set; } = new();
    
    // Tracks which configurations have associated games
    public Dictionary<Guid, bool> ConfigHasGames { get; set; } = new();

    // Form binding properties for creating new configuration
    [BindProperty]
    [Required(ErrorMessage = "Configuration name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be 1-50 characters")]
    public string ConfigName { get; set; } = "My Configuration";

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
    public bool IsCylindrical { get; set; } = false;

    // Properties for editing existing configuration
    [BindProperty]
    public Guid? EditingConfigId { get; set; }

    public bool IsEditing => EditingConfigId.HasValue;

    // Validation and status messages
    public string? ValidationError { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        LoadConfigurations();
    }

    public IActionResult OnPostCreate()
    {
        LoadConfigurations();

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

        // Create and save configuration
        var config = new GameConfiguration
        {
            Id = Guid.NewGuid(),
            Name = ConfigName,
            BoardWidth = BoardWidth,
            BoardHeight = BoardHeight,
            ConnectHow = ConnectHowMany,
            IsCylindrical = IsCylindrical
        };

        try
        {
            _configRepository.Save(config);
            SuccessMessage = $"Configuration '{ConfigName}' created successfully!";
            
            // Reset form to defaults
            ResetForm();
            LoadConfigurations();
        }
        catch (Exception ex)
        {
            ValidationError = $"Error saving configuration: {ex.Message}";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        try
        {
            await _configRepository.DeleteAsync(id);
            SuccessMessage = "Configuration deleted successfully!";
        }
        catch (Exception ex)
        {
            ValidationError = $"Error deleting configuration: {ex.Message}";
        }

        LoadConfigurations();
        return Page();
    }

    public IActionResult OnPostLoadForEdit(string id)
    {
        LoadConfigurations();

        try
        {
            if (!Guid.TryParse(id, out var configId))
            {
                ValidationError = "Invalid configuration ID format.";
                return Page();
            }
            
            // Check if this configuration has any associated games
            if (ConfigHasGames.TryGetValue(configId, out var hasGames) && hasGames)
            {
                ValidationError = "Cannot edit this configuration because it has one or more saved games associated with it. Delete the associated games first.";
                return Page();
            }
            
            var config = _configRepository.Load(id);
            
            // Populate form with loaded configuration values
            EditingConfigId = config.Id;
            ConfigName = config.Name;
            BoardWidth = config.BoardWidth;
            BoardHeight = config.BoardHeight;
            ConnectHowMany = config.ConnectHow;
            IsCylindrical = config.IsCylindrical;
        }
        catch (Exception ex)
        {
            ValidationError = $"Error loading configuration: {ex.Message}";
        }

        return Page();
    }

    public IActionResult OnPostUpdate()
    {
        if (!EditingConfigId.HasValue)
        {
            ValidationError = "No configuration is being edited.";
            LoadConfigurations();
            return Page();
        }
        
        LoadConfigurations();

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

        try
        {
            var config = _configRepository.Load(EditingConfigId.Value.ToString());
            config.Name = ConfigName;
            config.BoardWidth = BoardWidth;
            config.BoardHeight = BoardHeight;
            config.ConnectHow = ConnectHowMany;
            config.IsCylindrical = IsCylindrical;
            
            _configRepository.Save(config);
            SuccessMessage = $"Configuration '{ConfigName}' updated successfully!";
            
            // Exit edit mode and reset form
            EditingConfigId = null;
            ResetForm();
            LoadConfigurations();
        }
        catch (Exception ex)
        {
            ValidationError = $"Error updating configuration: {ex.Message}";
        }

        return Page();
    }

    public IActionResult OnPostCancelEdit()
    {
        EditingConfigId = null;
        ResetForm();
        LoadConfigurations();
        SuccessMessage = "Edit cancelled.";
        return Page();
    }

    private void LoadConfigurations()
    {
        SavedConfigurations.Clear();
        ConfigHasGames.Clear();
        var configList = _configRepository.List();
        
        // First, load all games to check which configs have associated games
        var gamesWithConfigs = new HashSet<Guid>();
        var gameList = _gameRepository.List();
        foreach (var (gameId, _) in gameList)
        {
            try
            {
                var game = _gameRepository.Load(gameId);
                if (game.GameConfigurationId != Guid.Empty)
                {
                    gamesWithConfigs.Add(game.GameConfigurationId);
                }
            }
            catch
            {
                // Skip games that can't be loaded
            }
        }
        
        foreach (var (id, description) in configList)
        {
            try
            {
                var config = _configRepository.Load(id);
                SavedConfigurations.Add(config);
                
                // Check if this configuration has any associated games
                var hasGames = gamesWithConfigs.Contains(config.Id);
                ConfigHasGames[config.Id] = hasGames;
            }
            catch
            {
                // Skip configurations that can't be loaded
            }
        }
        
        // Sort by name
        SavedConfigurations = SavedConfigurations.OrderBy(c => c.Name).ToList();
    }

    private void ResetForm()
    {
        ConfigName = "My Configuration";
        BoardWidth = 7;
        BoardHeight = 6;
        ConnectHowMany = 4;
        IsCylindrical = false;
    }
}
