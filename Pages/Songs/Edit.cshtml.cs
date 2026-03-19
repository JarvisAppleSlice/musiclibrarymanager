using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Songs;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<EditModel> _logger;

    public EditModel(AppDbContext context, ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public Song Song { get; set; } = new();

    public List<SelectListItem> ArtistOptions { get; set; } = new();
    public List<SelectListItem> AlbumOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                _logger.LogWarning("No song found with ID {SongId}", id);
                return NotFound();
            }

            Song = song;

            ArtistOptions = await _context.Artists
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToListAsync();

            AlbumOptions = await _context.Albums
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToListAsync();

            _logger.LogInformation("Editing song with ID {SongId}", id);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit page for song ID {SongId}", id);
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during song edit.");
            return Page();
        }

        try
        {
            _context.Attach(Song).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Song with ID {SongId} updated successfully.", Song.Id);

            TempData["StatusMessage"] = "Song successfully changed";

            return RedirectToPage("Details", new { id = Song.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating song with ID {SongId}", Song.Id);
            ModelState.AddModelError("", "An error occurred while updating the song.");
            return Page();
        }
    }
}