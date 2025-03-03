using System.Diagnostics;

public class BackupService
{


    public List<BackupDto> GetBackups(int year, int month)
    {
        List<string> files = GetAllFiles();
        List<BackupDto> backups = files
            .Where(f => f.Contains($"{year}-{month:00}"))
            .Select(BackupDto.GetFromFileName)
            .ToList();
        
        backups.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));

        return backups;
    }

    public BackupDto? GetBackupByFileName(string fileName)
    {
        List<string> files = GetAllFiles();
        if (!files.Contains(fileName)) {
            return null;
        }

        return BackupDto.GetFromFileName(fileName);
    }

    public string SaveBackupFile(string fileBytes)
    {
        string fileName = $"uploaded_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
        File.WriteAllText($"{Config.BACKUP_DIRECTORY}/{fileName}", fileBytes);

        return fileName;
    }

    public string CreateBackup()
    {
        string fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
        string fileContent;

        // run bash script
        var process = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "bash",
                Arguments = $"-c \"mysqldump -h {Config.DB_HOST} -P {Config.DB_PORT} -u {Config.DB_USER} -p{Config.DB_PASS} {Config.DB_NAME}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
        fileContent = process.StandardOutput.ReadToEnd();
        process.Close();

        if (fileContent == "") {
            throw new Exception("Backup failed");
        }

        File.WriteAllText($"{Config.BACKUP_DIRECTORY}/{fileName}", fileContent);
        return fileName;
    }

    public void RestoreBackup(string file)
    {
        DropDbIfExistSql();
        CreateDbSql();
        RestoreDbSql(file);
    }

    public void DeleteBackup(string file)
    {
        File.Delete($"{Config.BACKUP_DIRECTORY}/{file}");
    }

    private List<string> GetAllFiles()
    {
        // read all files from BACKUP_DIRECTORY
        List<string> files = Directory.GetFiles(Config.BACKUP_DIRECTORY).ToList();
        return files.ConvertAll(f => f.Split("/").Last());
    }

    private void DropDbIfExistSql() {
        // run bash script
        var process = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "bash",
                Arguments = $"-c \"mysql -h {Config.DB_HOST} -P {Config.DB_PORT} -u {Config.DB_USER} -p{Config.DB_PASS} -e 'DROP DATABASE IF EXISTS `{Config.DB_NAME}`'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        // check if restore was successful
        string ExitCode = process.ExitCode.ToString();
        if (ExitCode != "0") {
            throw new Exception("Drop db error");
        }

        process.Close();
    }

    private void CreateDbSql() {
        // run bash script
        var process = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "bash",
                Arguments = $"-c \"mysql -h {Config.DB_HOST} -P {Config.DB_PORT} -u {Config.DB_USER} -p{Config.DB_PASS} -e 'CREATE DATABASE `{Config.DB_NAME}`'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        // check if restore was successful
        string ExitCode = process.ExitCode.ToString();
        if (ExitCode != "0") {
            throw new Exception("Drop db error");
        }

        process.Close();
    }

    private void RestoreDbSql(string file) {
        // run bash script
        var process = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = "bash",
                Arguments = $"-c \"mysql -h {Config.DB_HOST} -P {Config.DB_PORT} -u {Config.DB_USER} -p{Config.DB_PASS} {Config.DB_NAME} < {Config.BACKUP_DIRECTORY}/{file}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        // check if restore was successful
        string ExitCode = process.ExitCode.ToString();
        if (ExitCode != "0") {
            throw new Exception("Drop db error");
        }

        process.Close();
    }
}