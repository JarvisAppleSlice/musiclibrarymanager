using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Data;
using MusicLibraryManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=music.db"));

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

// GET all songs
app.MapGet("/api/songs", async (AppDbContext db) =>
{
    return await db.Songs
        .Include(s => s.Artist)
        .Include(s => s.Album)
        .Select(s => new
        {
            s.Id,
            s.Title,
            s.ReleaseYear,
            s.Genre,
            ArtistId = s.ArtistId,
            ArtistName = s.Artist!.Name,
            AlbumId = s.AlbumId,
            AlbumName = s.Album!.Name
        })
        .ToListAsync();
});


// GET one song
app.MapGet("/api/songs/{id}", async (int id, AppDbContext db) =>
{
    var song = await db.Songs
        .Include(s => s.Artist)
        .Include(s => s.Album)
        .Where(s => s.Id == id)
        .Select(s => new
        {
            s.Id,
            s.Title,
            s.ReleaseYear,
            s.Genre,
            ArtistId = s.ArtistId,
            ArtistName = s.Artist!.Name,
            AlbumId = s.AlbumId,
            AlbumName = s.Album!.Name
        })
        .FirstOrDefaultAsync();

    return song is null ? Results.NotFound() : Results.Ok(song);
});


// POST create song
app.MapPost("/api/songs", async (Song song, AppDbContext db) =>
{
    db.Songs.Add(song);
    await db.SaveChangesAsync();

    return Results.Created($"/api/songs/{song.Id}", song);
});


// PUT update song
app.MapPut("/api/songs/{id}", async (int id, Song updatedSong, AppDbContext db) =>
{
    var song = await db.Songs.FindAsync(id);

    if (song == null)
        return Results.NotFound();

    song.Title = updatedSong.Title;
    song.Genre = updatedSong.Genre;
    song.ReleaseYear = updatedSong.ReleaseYear;
    song.ArtistId = updatedSong.ArtistId;
    song.AlbumId = updatedSong.AlbumId;

    await db.SaveChangesAsync();

    return Results.NoContent();
});


// DELETE song
app.MapDelete("/api/songs/{id}", async (int id, AppDbContext db) =>
{
    var song = await db.Songs.FindAsync(id);

    if (song == null)
        return Results.NotFound();

    db.Songs.Remove(song);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();