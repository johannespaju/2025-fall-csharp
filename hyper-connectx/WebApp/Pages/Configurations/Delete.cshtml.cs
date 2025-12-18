using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages.Configurations;

public class DeleteModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;

    public DeleteModel(IRepository<GameConfiguration> configRepository)
    {
        _configRepository = configRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public GameConfiguration? Configuration { get; set; }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("Index");
        }

        try
        {
            Configuration = _configRepository.Load(Id);
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

        try
        {
            _configRepository.Delete(Id);
        }
        catch
        {
            ErrorMessage = "Failed to delete the configuration.";
            try
            {
                Configuration = _configRepository.Load(Id);
            }
            catch
            {
                // Configuration no longer exists, redirect to index
                return RedirectToPage("Index");
            }
            return Page();
        }

        return RedirectToPage("Index");
    }
}
