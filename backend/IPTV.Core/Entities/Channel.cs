namespace IPTV.Core.Entities;

public class Channel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string StreamUrl { get; set; }
    public string? LogoUrl { get; set; }
    public int ChannelNumber { get; set; }
    public string? Category { get; set; }
    public string? Language { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public ICollection<EPGProgram> EPGPrograms { get; set; } = new List<EPGProgram>();
}
