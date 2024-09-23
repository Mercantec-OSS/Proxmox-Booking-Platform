namespace Models;

public class Task
{
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime StartTime { get; set; } 
    public DateTime EndTime { get; set; } 
    public TaskStatus Status { get; set; } = TaskStatus.Waiting;
    public string Output { get; set; } = string.Empty;
    public ICommand Command { get; set; } = null!;
    public string AfterThan { get; set; } = "";

    public enum TaskStatus
    {
        Waiting,
        Processing,
        Complete
    }
}
