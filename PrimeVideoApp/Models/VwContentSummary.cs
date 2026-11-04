using System;
using System.Collections.Generic;

namespace PrimeVideoApp.Models;

public partial class VwContentSummary
{
    public int? Id { get; set; }

    public string? SourceId { get; set; }

    public string? Title { get; set; }

    public string? ContentType { get; set; }

    public short? ReleaseYear { get; set; }

    public string? Rating { get; set; }

    public int? DurationValue { get; set; }

    public string? DurationUnit { get; set; }

    public long? CastCount { get; set; }

    public long? DirectorCount { get; set; }

    public long? GenreCount { get; set; }
}
