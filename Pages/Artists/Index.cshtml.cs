using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Artists;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Artist> Artists { get; set; } = new();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Artist Index page requested.");

        try
        {
            Artists = await _context.Artists
           .OrderBy(a => a.Name)
           .ToListAsync();


            _logger.LogInformation("Loaded {ArtistCount} artists.", Artists.Count);

            if (!Artists.Any())
            {
                _logger.LogWarning("No artists found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading index page.");

            ModelState.AddModelError("", "Something went wrong. Please try again.");
        }

    }
}