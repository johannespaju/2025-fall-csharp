using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL;
using DAL;

namespace WebApp.Pages.Configurations;

public class IndexModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepository;

    public IndexModel(IRepository<GameConfiguration> configRepository)
    {
        _configRepository = configRepository;
    }

    public List<(string Id, string Name, GameConfiguration Config)> Configurations { get; set; } = new();

    public void OnGet()
    {
        var configList = _configRepository.List();
        
        foreach (var (id, name) in configList)
        {
            try
            {
                var config = _configRepository.Load(id);
                Configurations.Add((id, name, config));
            }
            catch
            {
                // Skip configurations that can't be loaded
            }
        }
    }
}
