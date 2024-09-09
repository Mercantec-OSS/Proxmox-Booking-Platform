namespace Models;

[Table("groups")]
public class Group
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<User> Members {get; set;} = new();

    public GroupGetDto? MakeGetDto()
    {
        return new ()
        {
            Id=Id,
            Name=Name,
            Members=Members.ConvertAll(u=>u.MakeGetDto()),
            CreatedAt=DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc)
        };
    }
}
