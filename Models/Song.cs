using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MusicLibraryManager.Models;

public class Song
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(50)]
    public string Title { get; set; } = "";

    [Range(1900, 2026), Required(ErrorMessage = "Release Year is required.")]
    public int ReleaseYear { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    [MaxLength(50)]
    public string Genre { get; set; } = "";

    [Required(ErrorMessage = "Please select an artist.")]
    public int ArtistId { get; set; }
    public Artist? Artist { get; set; }

    [Required(ErrorMessage = "Please select an album.")]
    public int AlbumId { get; set; }
    public Album? Album { get; set; }
}