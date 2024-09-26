public static class FileWriter
{
    public static string WriteToFile(string content)
    {
        string randomFileName = Path.GetRandomFileName();
        string path = "/tmp/" + randomFileName;

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(content);
        }

        return path;
    }

    public static void RemoveFile(string path)
    {
        File.Delete(path);
    }
}