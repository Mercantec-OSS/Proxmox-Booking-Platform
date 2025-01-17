namespace Dto;

public class UserCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string InviteKey { get; set; } = string.Empty;
    public int? GroupId { get; set; } = null;
}
