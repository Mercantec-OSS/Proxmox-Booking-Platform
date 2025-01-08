using System.Text.RegularExpressions;


namespace Models;

public class ShellCommand : ICommand
{
    private readonly string command;

    public ShellCommand(string command = "")
    {
        this.command = command;
    }
    public string CommandString => Regex.Replace(command, @"'([^']*)'", "x");

    public string Execute(bool readOutput)
    {
        return ShellExecutorService.Execute(command, readOutput);
    }
}
