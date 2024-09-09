namespace VMwareBookingSystem.Models;

public class CommandModel
{
    public string Type { get; set; }
    public string Command { get; set; }
    public string? AfterThan { get; set; }
}
