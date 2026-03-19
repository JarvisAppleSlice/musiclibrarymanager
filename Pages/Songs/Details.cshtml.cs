using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Songs;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(AppDbContext context, ILogger<DetailsModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Song Song { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                _logger.LogWarning("Song not found with ID {SongId}", id);
                return NotFound();
            }

            Song = song;

            _logger.LogInformation("Viewing details for song with ID {SongId}", id);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving details for song with ID {SongId}", id);

            // Optional: user-friendly message
            ModelState.AddModelError("", "An error occurred while loading the song.");

            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}