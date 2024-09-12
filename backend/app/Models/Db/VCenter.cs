namespace Models;

[Table("vcenters")]
public class VCenter
{
    [Column("id")]
    public int Id { get; set; }

    [Column("booking_id")]
    public int? BookingId { get; set; }

    [ForeignKey("BookingId")]
    public ClusterBooking? ClusterBooking { get; set; }
    public List<EsxiHost> EsxiHosts { get; set; } = new ();

    [Column("ip")]
    public string Ip { get; set; } = "";

    [Column("user_name")]
    public string UserName { get; set; } = "";

    [Column("password")]
    public string Password { get; set; } = "";

    [Column("datacenter_name")]
    public string DatacenterName { get; set; } = "";

    [Column("cluster_name")]
    public string ClusterName { get; set; } = "";

    [Column("json_config")]
    public string JsonConfig { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public VCenterGetDto MakeGetDto()
    {
        return new()
        {
            Id = Id,
            Ip = Ip,
            UserName = UserName,
            Password = Password,
            BookingId = BookingId,
            EsxiHosts = EsxiHosts.Select(h => h.MakeGetDto()).ToList(),
            DatacenterName = DatacenterName,
            ClusterName = ClusterName,
            JsonConfig = JsonConfig,
            UpdatedAt = DateTime.SpecifyKind(UpdatedAt, DateTimeKind.Utc),
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc)
        };
    }
}
