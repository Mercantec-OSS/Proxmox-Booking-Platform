public static class VmCredentials
{
    public static string GetLoginByTemplate(string template)
    {
        string login;
        Config config = new();

        string preparedTemplate = template.ToLower();
        List<string> tokens = preparedTemplate.Split("_").ToList();

        // define for servers os
        if (tokens.Contains("server"))
        {
            if (tokens.Contains("windows"))
            {
                login = "Administrator";
            }
            else
            {
                login = "root";
            }
        }

        // Define for other os types 
        else
        {
            login = config.VM_DEFAULT_USER;
        }

        return login;
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