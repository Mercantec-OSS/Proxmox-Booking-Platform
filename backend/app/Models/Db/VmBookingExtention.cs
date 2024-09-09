namespace Models;

[Table("vm_booking_extentions")]
public class VmBookingExtention
{
    [Column("id")]
    public int Id { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public User? Owner { get; set; }

    [Column("assigned_id")]
    public int AssignedId { get; set; }
    [ForeignKey("AssignedId")]
    public User? Assigned { get; set; }

    [Column("booking_id")]
    [ForeignKey("booking_requests")]
    public int BookingId { get; set; }

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("is_accepted")]
    public bool IsAccepted { get; set; } = false;

    [Column("new_expired_at")]
    public DateTime NewExpiredAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("expired_at")]
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(3);

    public VmBookingExtentionGetDto MakeGetDTO()
    {
        return new VmBookingExtentionGetDto()
        {
            Id = Id,
            Owner = Owner?.MakeGetDto(),
            Assigned = Assigned?.MakeGetDto(),
            BookingId = BookingId,
            Message = Message,
            IsAccepted = IsAccepted,
            NewExpiringAt = DateTime.SpecifyKind(NewExpiredAt, DateTimeKind.Utc),
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(UpdatedAt, DateTimeKind.Utc),
            ExpiredAt = DateTime.SpecifyKind(ExpiredAt, DateTimeKind.Utc)
        };
    }
}
