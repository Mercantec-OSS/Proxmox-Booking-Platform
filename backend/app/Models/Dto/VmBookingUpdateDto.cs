namespace Dto;

public class VmBookingUpdateDto
{
    public int Id { get; set; }
    public bool IsAccepted { get; set; } = false;
    public DateTime NewExpiringDate { get; set; }
}
