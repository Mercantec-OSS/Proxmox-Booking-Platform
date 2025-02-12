public static class VmCredentials
{
    public static string GetLoginByTemplate(List<string> tags)
    {
        if (!tags.Contains("psw"))
        {
            return "";
        }

        if (tags.Contains("lin") && tags.Contains("srv"))
        {
            return "root";
        }

        if (tags.Contains("win") && tags.Contains("srv"))
        {
            return "Administrator";
        }

        return Config.VM_DEFAULT_USER;
    }

    public static string GetPasswordByTemplate(List<string> tags)
    {
        if (!tags.Contains("psw"))
        {
            return "";
        }

        return GenerateRandomPassword();
    }

    public static string GenerateRandomPassword(int length = 6)
    {
        Random random = new Random();

        string letters = "abcdefghjkmnopqrstuvwxyz";
        string upperLetters = letters.ToUpper();
        string digits = "0123456789";

        List<char> passwordChars = new List<char>
        {
            letters[random.Next(letters.Length)],
            upperLetters[random.Next(upperLetters.Length)],
            digits[random.Next(digits.Length)]
        };

        string allChars = letters + upperLetters + digits;
        for (int i = passwordChars.Count; i < length; i++)
        {
            passwordChars.Add(allChars[random.Next(allChars.Length)]);
        }

        return new string(passwordChars.OrderBy(c => random.Next()).ToArray());
    }
}