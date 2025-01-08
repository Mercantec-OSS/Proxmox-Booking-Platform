namespace Models;

public interface ICommand
{
    string CommandString { get; }
    string Execute(bool readOutput);
}
