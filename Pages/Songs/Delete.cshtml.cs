using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Songs;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(AppDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public Song Song { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Song? song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                _logger.LogWarning("Delete requested for non-existent song ID {SongId}", id);
                return NotFound();
            }

            Song = song;
            _logger.LogInformation("Delete confirmation loaded for song ID {SongId}", id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for song ID {SongId}", id);
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                _logger.LogWarning("Delete attempted for non-existent song ID {SongId}", id);
                return NotFound();
            }
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Song with ID {SongId} deleted successfully.", id);

            TempData["StatusMessage"] = "Song Removed successfully";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting song with ID {SongId}", id);
            ModelState.AddModelError("", "An error occurred while deleting the song.");
            return Page();
        }
    }
}