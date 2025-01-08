public static class Config
{
    public static string VERSION => ParseVariable("VERSION");
    public static string DB_CONNECTION_STRING => ParseVariable("DB_CONNECTION_STRING");
    public static string JWT_SECRET => ParseVariable("JWT_SECRET");

    public static string SMTP_ADDRESS => ParseSMTPConnectionString()[0];
    public static string SMTP_PORT => ParseSMTPConnectionString()[1];
    public static string SMTP_USER => ParseSMTPConnectionString()[2];
    public static string SMTP_PASSWORD => ParseSMTPConnectionString()[3];
    public static string EMAIL_TEMPLATES_PATH => ParseVariable("EMAIL_TEMPLATES_PATH");

    public static string VM_DEFAULT_USER => ParseVariable("VM_DEFAULT_USER");
    public static string VM_ROOT_PASSWORD => ParseVariable("VM_ROOT_PASSWORD");

    public static string SCRIPTS_PATH => ParseVariable("SCRIPTS_PATH");
    public static string VM_VCENTER_IP => ParseVariable("VM_VCENTER_IP");
    public static string VM_VCENTER_USER => ParseVariable("VM_VCENTER_USER");
    public static string VM_VCENTER_PASSWORD => ParseVariable("VM_VCENTER_PASSWORD");
    public static string VM_CLUSTER_NAME => ParseVariable("VM_CLUSTER_NAME");
    public static string VM_DATASTORE_NAME => ParseVariable("VM_DATASTORE_NAME");

    private static string ParseVariable(string variableName) {
        string variable = Environment.GetEnvironmentVariable(variableName) ?? "";
        if (string.IsNullOrEmpty(variable)) {
            string errorMsg = $"{variableName} is not set";
            throw new Exception(errorMsg);
        }
        return variable;
    }

    private static string[] ParseSMTPConnectionString() {
        string smtpConnectionString = ParseVariable("SMTP_CONNECTION_STRING");
        string[] stringArgs = smtpConnectionString.Split("__");

        if (stringArgs.Length != 4) {
            string errorMsg = $"SMTP_CONNECTION_STRING is not in the correct format: '{smtpConnectionString}'";
            throw new Exception(errorMsg);
        }

        return stringArgs;
    }
}