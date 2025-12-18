using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages.Configurations;

public class EditModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;

    public EditModel(IRepository<GameConfiguration> configRepository)
    {
        _configRepository = configRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Configuration name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be 1-100 characters")]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    [Range(3, 20, ErrorMessage = "Board width must be between 3 and 20")]
    public int BoardWidth { get; set; } = 7;

    [BindProperty]
    [Range(3, 20, ErrorMessage = "Board height must be between 3 and 20")]
    public int BoardHeight { get; set; } = 6;

    [BindProperty]
    [Range(2, 20, ErrorMessage = "Connect How Many must be between 2 and 20")]
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

    public string? ValidationError { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("Index");
        }

        try
        {
            var config = _configRepository.Load(Id);
            
            Name = config.Name;
            BoardWidth = config.BoardWidth;
            BoardHeight = config.BoardHeight;
            ConnectHowMany = config.ConnectHow;
            Player1Name = config.P1Name;
            Player2Name = config.P2Name;
            GameMode = config.Mode;
            IsCylindrical = config.IsCylindrical;
        }
        catch
        {
            return RedirectToPage("Index");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("Index");
        }

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
            var config = _configRepository.Load(Id);
            
            config.Name = Name;
            config.BoardWidth = BoardWidth;
            config.BoardHeight = BoardHeight;
            config.ConnectHow = ConnectHowMany;
            config.P1Name = Player1Name;
            config.P2Name = Player2Name;
            config.Mode = GameMode;
            config.IsCylindrical = IsCylindrical;

            _configRepository.Save(config);
        }
        catch
        {
            ValidationError = "Failed to save the configuration.";
            return Page();
        }

        return RedirectToPage("Index");
    }
}
