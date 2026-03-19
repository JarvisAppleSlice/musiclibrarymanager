using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Pages.Artists;

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
    public Artist Artist { get; set; } = new();

    public void OnGet()
    {
        _logger.LogInformation("Artist Create page requested.");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid during artist creation.");
            return Page();
        }

        try
        {
            _context.Artists.Add(Artist);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Artist with ID {ArtistId} created successfully.", Artist.Id);

            TempData["StatusMessage"] = "Artist added successfully!";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating artist.");

            ModelState.AddModelError("", "An error occurred while creating the artist.");
            return Page();
        }

    }
}