[Table("vm_bookings")]
public class VmBooking
{
    [Column("id")]
    public int Id { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public User Owner { get; set; } = null!;

    [Column("assigned_id")]
    public int AssignedId { get; set; }
    [ForeignKey("AssignedId")]
    public User Assigned { get; set; } = null!;
    public List<VmBookingExtention> Extentions { get; set; } = new ();

    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [Column("is_accepted")]
    public bool IsAccepted { get; set; } = false;

    [Column("uuid")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("expired_at")]
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow;

    public VmBookingGetDto MakeGetDTO()
    {
        return new()
        {
            Id = Id,
            Owner = Owner?.MakeGetDto(),
            Assigned = Assigned?.MakeGetDto(),
            Message = Message,
            Type = Type,
            Login = Login,
            Password = Password,
            IsAccepted = IsAccepted,
            Uuid = Name,
            Extentions = Extentions.ConvertAll(e => e.MakeGetDTO()),
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(UpdatedAt, DateTimeKind.Utc),
            ExpiredAt = DateTime.SpecifyKind(ExpiredAt, DateTimeKind.Utc),
        };
    }
}
