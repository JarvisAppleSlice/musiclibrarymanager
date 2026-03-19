using Microsoft.EntityFrameworkCore;
using MusicLibraryManager.Models;

namespace MusicLibraryManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();

    public DbSet<Song> Songs => Set<Song>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Artist>().HasData(
            new Artist { Id = 1, Name = "Bayside" },
            new Artist { Id = 2, Name = "Yanni" },
            new Artist { Id = 3, Name = "Sabaton" },
            new Artist { Id = 4, Name = "Three Days Grace" },
            new Artist { Id = 5, Name = "Five Finger Death Punch" }
        );

        modelBuilder.Entity<Album>().HasData(
           new Album { Id = 1, Name = "Killing Time", ReleaseYear = 2011, ArtistId = 1 },
           new Album { Id = 2, Name = "Reflections of Passion", ReleaseYear = 1990, ArtistId = 2 },
           new Album { Id = 3, Name = "Sensuous Chill", ReleaseYear = 2016, ArtistId = 2 },
           new Album { Id = 4, Name = "The Last Stand", ReleaseYear = 2016, ArtistId = 3 },
           new Album { Id = 5, Name = "And Justice for None", ReleaseYear = 2018, ArtistId = 5 },
           new Album { Id = 6, Name = "Explosions", ReleaseYear = 2022, ArtistId = 4 }
       );
    }
}