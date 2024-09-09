[Table("cluster_bookings")]
public class ClusterBooking
{
    [Column("id")]
    public int Id { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public User Owner { get; set; } = null!;
    public List<VCenter> VCenters { get; set; } = new List<VCenter>();

    [Column("amount_students")]
    public int AmountStudents { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("expired_at")]
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow;

    public ClusterBookingGetDto MakeGetDto()
    {
        return new()
        {
            Id = Id,
            AmountStudents = AmountStudents,
            VCenters = VCenters.ConvertAll(v => v.MakeGetDto()),
            Owner = Owner?.MakeGetDto(),
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc),
            ExpiredAt = DateTime.SpecifyKind(ExpiredAt, DateTimeKind.Utc),
        };
    }
}
