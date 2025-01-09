public class ScriptService
{
    public string Execute(Script script, bool readOutput)
    {
        string output = "";

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = script.Interpreter,
            Arguments = $"-c \"{script.Command}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        using (Process process = new Process())
        {
            try
            {
                process.StartInfo = processStartInfo;
                process.Start();

                if (readOutput)
                {
                    output = process.StandardOutput.ReadToEnd();
                }
                else {
                    output = "Command was started";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing command: " + ex.Message);
                output = $"Error executing command: {ex.Message}";
            }
        }

        return output;
    }
}