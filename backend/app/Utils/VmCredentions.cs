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
    public static string GenerateRandomPassword()
    {
        byte[] bytes = new byte[3];
        RandomNumberGenerator.Fill(bytes);

        StringBuilder hash = new StringBuilder();
        foreach (byte b in bytes)
        {
            hash.Append(b.ToString("x2"));
        }

        return hash.ToString();
    }
}