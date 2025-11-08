namespace IPTV.Core.Entities;

public class ViewingHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? ContentId { get; set; }
    public int? ChannelId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; } // Duration in seconds
    public int? Progress { get; set; } // Progress in seconds (for resume functionality)
    public bool Completed { get; set; }
    public string? DeviceInfo { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Content? Content { get; set; }
    public Channel? Channel { get; set; }
}
