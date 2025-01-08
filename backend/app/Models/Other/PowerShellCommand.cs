using System.Text.RegularExpressions;


namespace Models;

public class PowerShellCommand : ICommand
{
    private readonly string command;

    public string CommandString => Regex.Replace(command, @"'([^']*)'", "x");

    public PowerShellCommand(string command = "")
    {
        this.command = command;
    }

    private enum OsType
    {
        Windows,
        Linux
    }

    private OsType GetOsType()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return OsType.Windows;
        }
        else
        {
            return OsType.Linux;
        }
    }

    public string Execute(bool readOutput)
    {
        OsType osType = GetOsType();

        switch (osType)
        {
            case OsType.Windows:
                return PowerShellExecutorService.ExecuteOnWindows(command, readOutput);

            case OsType.Linux:
                return PowerShellExecutorService.ExecuteOnLinux(command, readOutput);

            default:
                return "Unknown OS error";
        }
    }

    public static PowerShellCommand CreateVmCommand(string command)
    {
        return new PowerShellCommand(command);
    }
}
