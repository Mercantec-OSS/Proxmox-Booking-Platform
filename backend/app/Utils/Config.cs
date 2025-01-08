public class Config
{
    public string VERSION => ParseVariable("VERSION");
    public string DB_CONNECTION_STRING => ParseVariable("DB_CONNECTION_STRING");
    public string JWT_SECRET => ParseVariable("JWT_SECRET");

    public string SMTP_ADDRESS => ParseSMTPConnectionString()[0];
    public string SMTP_PORT => ParseSMTPConnectionString()[1];
    public string SMTP_USER => ParseSMTPConnectionString()[2];
    public string SMTP_PASSWORD => ParseSMTPConnectionString()[3];
    public string EMAIL_TEMPLATES_PATH => ParseVariable("EMAIL_TEMPLATES_PATH");

    public string VM_DEFAULT_USER => ParseVariable("VM_DEFAULT_USER");
    public string VM_ROOT_PASSWORD => ParseVariable("VM_ROOT_PASSWORD");

    public string SCRIPTS_PATH => ParseVariable("SCRIPTS_PATH");
    public string VM_VCENTER_IP => ParseVariable("VM_VCENTER_IP");
    public string VM_VCENTER_USER => ParseVariable("VM_VCENTER_USER");
    public string VM_VCENTER_PASSWORD => ParseVariable("VM_VCENTER_PASSWORD");
    public string VM_CLUSTER_NAME => ParseVariable("VM_CLUSTER_NAME");
    public string VM_DATASTORE_NAME => ParseVariable("VM_DATASTORE_NAME");

    private string ParseVariable(string variableName) {
        string variable = Environment.GetEnvironmentVariable(variableName) ?? "";
        if (string.IsNullOrEmpty(variable)) {
            string errorMsg = $"{variableName} is not set";
            throw new Exception(errorMsg);
        }
        return variable;
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