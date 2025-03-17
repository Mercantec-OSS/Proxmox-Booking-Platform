[ApiController]
[Route("reset-password")]
public class ResetPasswordController(
    UserRepository userRepository,
    EmailService emailService
    ) : ControllerBase
{
    [HttpPost("create-token")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> CreateResetToken(ResetPasswordCreateDto resetPasswordGetDto)
    {
        User? user = await userRepository.GetAsync(resetPasswordGetDto.Email);
        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        ResetPasswordDto token = ResetPasswordService.CreateToken(resetPasswordGetDto.Email);
        EmailDto email = EmailDto.GetResetPassword(resetPasswordGetDto.Email, token.Token);
        await emailService.SendAsync(email);

        return NoContent();
    }

    [HttpPost("validate-token")]
    [ProducesResponseType(200)]
    public ActionResult<bool> ValidateResetToken(ResetPasswordValidateDto resetPasswordValidateDto)
    {
        ResetPasswordDto? token = ResetPasswordService.GetToken(resetPasswordValidateDto.Email, resetPasswordValidateDto.Token);
        if (token == null)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Token not found or expired. Check your email or request new token."));
        }

        return Ok(true);
    }

    [HttpPost("change-password")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> UseResetToken(ResetPasswordChangeDto resetPasswordChangeDto)
    {
        bool isAccepted = ResetPasswordService.UseToken(resetPasswordChangeDto.Email, resetPasswordChangeDto.Token);
        if (!isAccepted)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Token not found or expired. Check your email or request new token."));
        }

        User? user = await userRepository.GetAsync(resetPasswordChangeDto.Email);
        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        user.Password = Password.GetHash(resetPasswordChangeDto.Password);
        await userRepository.UpdateAsync(user);

        return NoContent();
    }

}
