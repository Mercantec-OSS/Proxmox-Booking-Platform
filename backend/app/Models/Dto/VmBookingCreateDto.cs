namespace Dto;

public class VmBookingCreateDto
{
    public int? OwnerId { get; set; }
    public int? AssignedId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ExpiringAt { get; set; }
    public string Type { get; set; } = string.Empty;
}
