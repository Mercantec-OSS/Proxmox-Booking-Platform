namespace Dto;

public class VmBookingExtentionCreateDto
{
    public int BookingId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime NewExpiringAt { get; set; }
}
