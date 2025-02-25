public static class Config
{
    public static string ACCESS_PASS => ParseVariable("ACCESS_PASS");
    public static string DB_HOST => ParseVariable("DB_HOST");
    public static string DB_PORT => ParseVariable("DB_PORT");
    public static string DB_USER => ParseVariable("DB_USER");
    public static string DB_PASS => ParseVariable("DB_PASS");
    public static string DB_NAME => ParseVariable("DB_NAME");
    public static string BACKUP_DIRECTORY => ParseVariable("BACKUP_DIRECTORY");


    private static string ParseVariable(string variableName) {
        string variable = Environment.GetEnvironmentVariable(variableName) ?? "";
        if (string.IsNullOrEmpty(variable)) {
            string errorMsg = $"{variableName} is not set";
            throw new Exception(errorMsg);
        }
        return variable;
    }

}