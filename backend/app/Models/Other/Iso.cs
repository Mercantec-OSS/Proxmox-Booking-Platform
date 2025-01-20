namespace Models;

public class Iso
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";

    public static Iso GetFromScriptLine(string line)
    {
        string[] parts = line.Split("||||");
        if (parts.Length == 2)
        {
            return new Iso()
            {
                Name = parts[0],
                Path = parts[1]
            };
        }
        return new Iso();
    }
}