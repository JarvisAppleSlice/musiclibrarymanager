using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MusicLibraryManager.Models;

public class Album
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(50)]
    public string Name { get; set; } = "";

    [Range(1900, 2026), Required(ErrorMessage = "Release Year is required.")]
    public int ReleaseYear { get; set; }

    [Required(ErrorMessage = "Please select an artist.")]
    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }

    public List<Song> Songs { get; set; } = new();
}