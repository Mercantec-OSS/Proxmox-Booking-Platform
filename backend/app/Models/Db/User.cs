namespace Models;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("first_name")]
    public string Name { get; set; } = "";

    [Column("surname")]
    public string Surname { get; set; } = "";

    [Column("email")]
    public string Email { get; set; } = "";

    [Column("pass")]
    public string Password { get; set; } = "";

    [Column("role")]
    public string Role { get; set; } = UserRoles.Student.ToString();

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public enum UserRoles
    {
        Student,
        Teacher,
        Moderator,
        Admin
    }

    public UserGetDto MakeGetDto() {
        return new()
        {
            Id = Id,
            Name = Name,
            Surname = Surname,
            Email = Email,
            Role = Role,
            CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc)
        };
    }

    public bool IsAdmin() => Role == UserRoles.Admin.ToString();
    public bool IsTeacher() => Role == UserRoles.Teacher.ToString();
    public bool IsModerator() => Role == UserRoles.Moderator.ToString();
    public bool IsStudent() => Role == UserRoles.Student.ToString();
}
