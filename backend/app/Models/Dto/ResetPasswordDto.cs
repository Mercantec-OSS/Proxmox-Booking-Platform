namespace Dto;

public class ResetPasswordDto {
    public string Email { get; set; } = "";
    public string Token { get; set; } = "";
    public DateTime ExpireAt { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(1);

    public bool IsValid() {
        return DateTime.UtcNow < ExpireAt;
    }

    public static ResetPasswordDto Create(string email) {
        return new ResetPasswordDto {
            Email = email,
            Token = Helpers.GetRandomNumber().ToString(),
            ExpireAt = DateTime.UtcNow + TimeSpan.FromMinutes(15)
        };
    }
}