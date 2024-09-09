public static class Password
{
    public static string GetHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool Verify(string password, string correctHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, correctHash);
    }
}