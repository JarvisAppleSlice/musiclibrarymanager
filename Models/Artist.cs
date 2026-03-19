using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MusicLibraryManager.Models;

public class Artist
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Artist name is required.")]
    [MaxLength(50)]
    public string Name { get; set; } = "";

    public List<Album> Albums { get; set; } = new();
    public List<Song> Songs { get; set; } = new();
}