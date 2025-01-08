namespace Services;

public static class PowerShellExecutorService
{
    public static string ExecuteOnWindows(string command, bool readOutput)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-c \"{command}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                if (readOutput)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    return output;
                }
                return "Command was started";
            }
        }
        catch (Exception ex)
        {
            return $"Error executing command: {ex.Message}";
        }
    }

    public static string ExecuteOnLinux(string command, bool readOutput)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/pwsh",
                Arguments = $"-c \"{command}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                if (readOutput)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    return output;
                }
                return "Command was started";
            }
        }
        catch (Exception ex)
        {
            return $"Error executing command: {ex.Message}";
        }
    }
}
