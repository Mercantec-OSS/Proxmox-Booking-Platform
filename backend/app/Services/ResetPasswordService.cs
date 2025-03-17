public class ResetPasswordService() : BackgroundService
{
    private static List<ResetPasswordDto> tokens = new List<ResetPasswordDto>();
    private static DateTime? lastUpdate = null;

    public static ResetPasswordDto CreateToken(string email)
    {
        ResetPasswordDto token = ResetPasswordDto.Create(email);

        tokens.Add(token);
        return token;
    }


    public static ResetPasswordDto? GetToken(string email, string token)
    {
        ResetPasswordDto? selectedToken = tokens.Find(t => t.Email.ToLower() == email.ToLower() && t.Token == token && t.IsValid());
        if (selectedToken == null)
        {
            return null;
        }

        return selectedToken;
    }

    public static bool UseToken(string email, string token)
    {
        ResetPasswordDto? selectedToken = GetToken(email, token);
        if (selectedToken == null)
        {
            return false;
        }

        tokens.Remove(selectedToken);
        return true;
    }

    private void DeleteExpiredTokens()
    {
        lastUpdate = DateTime.UtcNow;
        tokens.RemoveAll(token => token.ExpireAt < lastUpdate);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Delete expired tokens
            DeleteExpiredTokens();

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}