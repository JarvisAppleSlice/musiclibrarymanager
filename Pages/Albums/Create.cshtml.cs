using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Albums;

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
    public Album Album { get; set; } = new();

    public List<SelectListItem> ArtistOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Create album request received.");

        try
        {
            ArtistOptions = await _context.Artists
           .Select(a => new SelectListItem
           {
               Value = a.Id.ToString(),
               Text = a.Name
           })
           .ToListAsync();

            _logger.LogInformation("Create album page loaded successfully.");
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
            _logger.LogWarning("Model state invalid during album creation.");
            await OnGetAsync();
            return Page();
        }
        try
        {
            _context.Albums.Add(Album);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Album with ID {AlbumId} created successfully.", Album.Id);

            TempData["StatusMessage"] = "Album added successfully!";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding new album.");
            ModelState.AddModelError("", "An unexpected error occurred while adding the album.");
            await OnGetAsync();
            return Page();
        }
    }
}