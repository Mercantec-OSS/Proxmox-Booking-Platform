public class InviteKeysService() : BackgroundService
{
    private static List<InviteToken> tokens = new ();
    private static DateTime? lastUpdate = null;

    public static InviteToken CreateToken(string email, string role)
    {
        InviteToken token = InviteToken.Create(email, role);

        tokens.Add(token);
        return token;
    }


    public static InviteToken? GetToken(string email, string token)
    {
        InviteToken? selectedToken = tokens.Find(t => t.Email.ToLower() == email.ToLower() && t.Token == token && t.IsValid());
        if (selectedToken == null)
        {
            return null;
        }

        return selectedToken;
    }

    public static InviteToken? UseToken(string email, string token)
    {
        InviteToken? selectedToken = GetToken(email, token);
        if (selectedToken == null)
        {
            return null;
        }

        tokens.Remove(selectedToken);
        return selectedToken;
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