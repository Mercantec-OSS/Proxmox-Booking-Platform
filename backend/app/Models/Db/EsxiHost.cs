namespace Models;

[Table("esxi_hosts")]
public class EsxiHost
{
    [Column("id")]
    public int Id { get; set; }

    [Column("vcenter_id")]
    public int VCenterId { get; set; }
    [ForeignKey("VCenterId")]
    public VCenter? VCenter { get; set; }

    [Column("ip")]
    public string Ip { get; set; } = "";

    [Column("user_name")]
    public string UserName { get; set; } = "";

    [Column("password")]
    public string Password { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public EsxiHostGetDto MakeGetDto()
    {
        return new ()
        {
            Id = Id,
            VCenterId = VCenter?.Id,
            BookingId = VCenter?.BookingId,
            Ip = Ip,
            UserName = UserName,
            Password = Password,
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(UpdatedAt, DateTimeKind.Utc)
        };
    }
}
