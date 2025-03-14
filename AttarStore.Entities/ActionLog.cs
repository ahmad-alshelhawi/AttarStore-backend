using AttarStore.Entities;

public class ActionLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public User User { get; set; }
}
