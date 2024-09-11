public class Config
{
    public string VERSION => ParseVariable("VERSION");
    public bool TEST_MODE => ParseVariable("TEST_MODE").ToLower() == "true";
    public string DB_CONNECTION_STRING => ParseVariable("DB_CONNECTION_STRING");
    public string JWT_SECRET => ParseVariable("JWT_SECRET");
    public string DEVOPS_URL => ParseVariable("DEVOPS_URL");

    public string AD_ADDRESS => ParseAdConnectionString()[0];
    public string AD_DOMAIN => ParseAdConnectionString()[1];
    public string AD_USER => ParseAdConnectionString()[2];
    public string AD_PASSWORD => ParseAdConnectionString()[3];
   

    public string SMTP_ADDRESS => ParseSMTPConnectionString()[0];
    public string SMTP_PORT => ParseSMTPConnectionString()[1];
    public string SMTP_USER => ParseSMTPConnectionString()[2];
    public string SMTP_PASSWORD => ParseSMTPConnectionString()[3];

    public string VM_DEFAULT_USER => ParseVariable("VM_DEFAULT_USER");
    public string VM_ROOT_PASSWORD => ParseVariable("VM_ROOT_PASSWORD");

    private string ParseVariable(string variableName) {
        string variable = Environment.GetEnvironmentVariable(variableName) ?? "";
        if (string.IsNullOrEmpty(variable)) {
            string errorMsg = $"{variableName} is not set";
            throw new Exception(errorMsg);
        }
        return variable;
    }

    private string[] ParseAdConnectionString() {
        string adConnectionString = ParseVariable("AD_CONNECTION_STRING");
        string[] stringArgs = adConnectionString.Split("__");

        if (stringArgs.Length != 4) {
            string errorMsg = $"AD_CONNECTION_STRING is not in the correct format: '{adConnectionString}'";
            throw new Exception(errorMsg);
        }

        return stringArgs;
    }

    private string[] ParseSMTPConnectionString() {
        string smtpConnectionString = ParseVariable("SMTP_CONNECTION_STRING");
        string[] stringArgs = smtpConnectionString.Split("__");

        if (stringArgs.Length != 4) {
            string errorMsg = $"SMTP_CONNECTION_STRING is not in the correct format: '{smtpConnectionString}'";
            throw new Exception(errorMsg);
        }

        return stringArgs;
    }
}