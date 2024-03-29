﻿using dominikz.Domain.Enums.Movies;

namespace dominikz.Domain.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public string? Comment { get; set; }
    public MovieGenresFlags Genres { get; set; }
    public int Rating { get; set; }
    public int Year { get; set; }
    public string Plot { get; set; } = string.Empty;
    public TimeSpan Runtime { get; set; }
    public string ImdbId { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? TrailerPath { get; set; }
}