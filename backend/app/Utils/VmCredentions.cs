public static class VmCredentials
{
    public static string GetLoginByTemplate(TemplateGetDto template)
    {
        Config config = new();

        if (!template.Keywords.Contains("psw"))
        {
            return "";
        }

        if (template.Keywords.Contains("lin") && template.Keywords.Contains("srv"))
        {
            return "root";
        }

        if (template.Keywords.Contains("win") && template.Keywords.Contains("srv"))
        {
            return "Administrator";
        }

        return config.VM_DEFAULT_USER;
    }

    public static string GetPasswordByTemplate(TemplateGetDto template)
    {
        if (!template.Keywords.Contains("psw"))
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