using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Albums;

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
    public Album Album { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                _logger.LogWarning("Delete requested for non-existent album ID {AlbumId}", id);
                return NotFound();
            }

            Album = album;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for album ID {AlbumId}", id);
            return StatusCode(500);
        }

    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var album = await _context.Albums
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                _logger.LogWarning("Delete attempted for non-existent album ID {AlbumId}", id);
                return NotFound();
            }

            if (album.Songs.Any())
            {
                ModelState.AddModelError("",
                    "Cannot delete this album. Delete the songs first.");

                Album = album;

                return Page();
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Album with ID {AlbumId} deleted successfully.", id);

            TempData["StatusMessage"] = "Album Removed successfully";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting album with ID {AlbumId}", id);
            ModelState.AddModelError("", "An error occurred while deleting the album.");
            return Page();
        }
    }
}