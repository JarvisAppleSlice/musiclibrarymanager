using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Artists;

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
    public Artist Artist { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {

        try
        {
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                _logger.LogWarning("No artist found with ID {ArtistId}", id);
                return NotFound();
            }

            Artist = artist;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit page for artist ID {ArtistId}", id);
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during artist edit.");
            return Page();
        }
        try
        {
            _context.Attach(Artist).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Artist updated successfully";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating song with ID {ArtistId}", Artist.Id);
            ModelState.AddModelError("", "An error occurred while updating the Artist.");
            return Page();
        }
    }
}