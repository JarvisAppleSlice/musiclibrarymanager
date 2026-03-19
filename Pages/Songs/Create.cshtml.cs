using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Songs;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AppDbContext context, ILogger<CreateModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public Song Song { get; set; } = new();

    public List<SelectListItem> ArtistOptions { get; set; } = new();
    public List<SelectListItem> AlbumOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Create song request received.");

        try
        {
            ArtistOptions = await _context.Artists
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            AlbumOptions = await _context.Albums
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            _logger.LogInformation("Create song page loaded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Create page options.");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during song creation.");
            await OnGetAsync();
            return Page();
        }

        try
        {
            _context.Songs.Add(Song);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Song with ID {SongId} created successfully.", Song.Id);

            TempData["StatusMessage"] = "Song added successfully!";

            return RedirectToPage("Details", new { id = Song.Id });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error adding new song.");
            ModelState.AddModelError("", "A database error occurred while adding the song.");
            await OnGetAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding new song.");
            ModelState.AddModelError("", "An unexpected error occurred while adding the song.");
            await OnGetAsync();
            return Page();
        }
    }
}