using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages;

public class ConfigManagerModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;

    public ConfigManagerModel(IRepository<GameConfiguration> configRepository)
    {
        _configRepository = configRepository;
    }

    // List of saved configurations
    public List<GameConfiguration> SavedConfigurations { get; set; } = new();

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
            P1Name = Player1Name,
            P2Name = Player2Name,
            Mode = GameMode,
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
            var config = _configRepository.Load(id);
            
            // Populate form with loaded configuration values
            ConfigName = config.Name;
            BoardWidth = config.BoardWidth;
            BoardHeight = config.BoardHeight;
            ConnectHowMany = config.ConnectHow;
            Player1Name = config.P1Name;
            Player2Name = config.P2Name;
            GameMode = config.Mode;
            IsCylindrical = config.IsCylindrical;
        }
        catch (Exception ex)
        {
            ValidationError = $"Error loading configuration: {ex.Message}";
        }

        return Page();
    }

    private void LoadConfigurations()
    {
        SavedConfigurations.Clear();
        var configList = _configRepository.List();
        
        foreach (var (id, description) in configList)
        {
            try
            {
                var config = _configRepository.Load(id);
                SavedConfigurations.Add(config);
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
        Player1Name = "Player 1";
        Player2Name = "Player 2";
        GameMode = EGameMode.PvP;
        IsCylindrical = false;
    }
}
