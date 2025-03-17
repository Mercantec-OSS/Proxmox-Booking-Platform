namespace Dto;

public class ResetPasswordChangeDto {
    public string Email { get; set; } = "";
    public string Token { get; set; } = "";
    public string Password { get; set; } = "";
}