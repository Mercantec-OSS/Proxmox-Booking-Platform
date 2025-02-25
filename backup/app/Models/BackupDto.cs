public class BackupDto
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static BackupDto GetFromFileName(string name) {
        // Example "backup_2025-02-19_11-11-50.sql"
        string dateTimePart = name.Replace("backup_", "").Replace("uploaded_", "").Replace(".sql", "");
        DateTime createdAt = DateTime.ParseExact(dateTimePart, "yyyy-MM-dd_HH-mm-ss", null);

        return new BackupDto {
            FileName = name,
            CreatedAt = createdAt
        };
    }
}