using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Artists;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(AppDbContext context, ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    // [BindProperty]
    public Artist Artist { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                _logger.LogWarning("Delete requested for non-existent artist ID {ArtistId}", id);
                return NotFound();
            }

            Artist = artist;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for artist ID {ArtistId}", id);

            return StatusCode(500);
        }

    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var artist = await _context.Artists
           .Include(a => a.Albums)
           .Include(a => a.Songs)
           .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                _logger.LogWarning("Delete attempted for non-existent artist ID {ArtistId}", id);
                return NotFound();
            }

            if (artist.Albums.Any() || artist.Songs.Any())
            {
                ModelState.AddModelError("",
                    "Cannot delete this artist. Delete albums and songs first.");

                Artist = artist;

                return Page();
            }

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Artist with ID {ArtistId} deleted successfully.", id);

            TempData["StatusMessage"] = "Artist Removed successfully";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting artist with ID {ArtistId}", id);
            ModelState.AddModelError("", "An error occurred while deleting the artist.");
            return Page();
        }
    }
}