using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages.Configurations;

public class CreateModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;

    public CreateModel(IRepository<GameConfiguration> configRepository)
    {
        _configRepository = configRepository;
    }

    [BindProperty]
    [Required(ErrorMessage = "Configuration name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be 1-100 characters")]
    public string Name { get; set; } = "My Configuration";

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

    public void OnGet()
    {
        // Initialize with defaults - already set in property declarations
    }

    public IActionResult OnPost()
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

        var config = new GameConfiguration
        {
            Id = Guid.NewGuid(),
            Name = Name,
            BoardWidth = BoardWidth,
            BoardHeight = BoardHeight,
            ConnectHow = ConnectHowMany,
            P1Name = Player1Name,
            P2Name = Player2Name,
            Mode = GameMode,
            IsCylindrical = IsCylindrical
        };

        _configRepository.Save(config);

        return RedirectToPage("Index");
    }
}
