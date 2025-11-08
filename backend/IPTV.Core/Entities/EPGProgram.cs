namespace IPTV.Core.Entities;

public class EPGProgram
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Category { get; set; }
    public string? Rating { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Channel? Channel { get; set; }
}
