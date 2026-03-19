using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Albums;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Album> Albums { get; set; } = new();

    public async Task OnGetAsync(int? artistId)
    {
        _logger.LogInformation("Albums Index page requested.");

        try
        {
            IQueryable<Album> query = _context.Albums
        .Include(a => a.Artist);

            if (artistId != null)
            {
                query = query.Where(a => a.ArtistId == artistId);
            }

            Albums = await query.ToListAsync();

            _logger.LogInformation("Loaded {AlbumCount} albums.", Albums.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading index page.");

            ModelState.AddModelError("", "Something went wrong. Please try again.");
        }
    }
}