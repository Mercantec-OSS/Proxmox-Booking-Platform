namespace VMwareBookingSystem.Services;

public static class ShellExecutorService
{
    public static string Execute(string command, bool readOutput)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
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
            Console.WriteLine("Error executing command: " + ex.Message);
            return $"Error executing command: {ex.Message}";
        }
    }
}
