using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Songs;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ArtistId { get; set; }

    public List<Artist> Artists { get; set; } = new();


    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Song> Songs { get; set; } = new();

    public async Task OnGetAsync(int? albumId)
    {
        _logger.LogInformation("Songs Index page requested.");

        try
        {

            Artists = await _context.Artists
            .OrderBy(a => a.Name)
            .ToListAsync();

            IQueryable<Song> query = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album);

            if (albumId != null)
            {
                query = query.Where(s => s.AlbumId == albumId);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                _logger.LogInformation("Songs search requested: {SearchTerm}", SearchTerm);

                var search = SearchTerm.ToLower();

                query = query.Where(s => s.Title.Contains(SearchTerm));
            }

            if (ArtistId.HasValue)
            {
                query = query.Where(s => s.ArtistId == ArtistId);
            }

            Songs = await query
            .OrderBy(s => s.Title)
            .ToListAsync();

            _logger.LogInformation("Loaded {SongsCount} songs.", Songs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading index page.");

            ModelState.AddModelError("", "Something went wrong. Please try again.");
        }
    }
}