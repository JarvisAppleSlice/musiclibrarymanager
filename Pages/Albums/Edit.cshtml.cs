using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Albums;

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
    public Album Album { get; set; } = null!;

    public SelectList Artists { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                _logger.LogWarning("No album found with ID {AlbumId}", id);
                return NotFound();
            }

            Album = album;

            Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit page for album ID {AlbumId}", id);
            return StatusCode(500);
        }

    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid during song edit.");
                Artists = new SelectList(await _context.Artists.ToListAsync(), "Id", "Name");
                return Page();
            }

            _context.Attach(Album).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Album with ID {AlbumId} updated successfully.", Album.Id);

            TempData["StatusMessage"] = "Album updated successfully";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating album with ID {AlbumId}", Album.Id);
            ModelState.AddModelError("", "An error occurred while updating the album.");
            return Page();
        }
    }
}