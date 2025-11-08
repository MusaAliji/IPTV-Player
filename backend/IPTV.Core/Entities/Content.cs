namespace IPTV.Core.Entities;

public class Content
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string StreamUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public ContentType Type { get; set; }
    public int? Duration { get; set; } // Duration in seconds for VOD
    public DateTime? ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public double? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum ContentType
{
    LiveTV,
    VOD,
    Series,
    Movie
}
