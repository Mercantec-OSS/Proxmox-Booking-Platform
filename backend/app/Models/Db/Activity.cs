namespace Models;

public class Activity
{
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public int OwnerId { get; set; }
    public int BookingId { get; set; } = 0;
    public List<ActivityTask> TaskList { get; set; } = [];
    public ActivityEvent Action { get; set; }
    public ActivityType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public enum ActivityEvent
    {
        Create,
        Update,
        Delete,
    }

    public enum ActivityType
    {
        Booking,
        BookingRequest,
        ExtentionRequest,
        VCenter,
        EsxiHost,
    }

    public class ActivityTask
    {
        public string TaskName { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public int OwnerId { get; set; } = 0;
        public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
    }
}
