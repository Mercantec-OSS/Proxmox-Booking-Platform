using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;

namespace app.Controllers;

public class PanelController(UserSession userSession, BackupService backupService) : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        return View();
    }

    [HttpGet("/backups")]
    public ActionResult<BackupDto> Backups(int year, int month)
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        return Ok(backupService.GetBackups(year, month));
    }

    [HttpPost("/create-backup")]
    public ActionResult CreateBackup()
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        string fileName = backupService.CreateBackup();
        return Ok(fileName);
    }

    [HttpGet("/download-backup")]
    public IActionResult DownloadBackup(string file)
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        var backup = backupService.GetBackupByFileName(file);
        if (backup is null) {
            return NotFound("Backup not found");
        }

        // read file bytes
        byte[] fileBytes = System.IO.File.ReadAllBytes($"{Config.BACKUP_DIRECTORY}/{file}");
        return File(fileBytes, "application/sql", file);
    }

    [HttpPost("/upload-backup")]
    public IActionResult UploadBackup(IFormFile file)
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        if (file is null) {
            return BadRequest("File not found");
        }

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        string fileContent = reader.ReadToEnd();

        return Ok(backupService.SaveBackupFile(fileContent));
    }

    [HttpPost("/apply-backup")]
    public IActionResult ApplyBackup(string file)
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        var backup = backupService.GetBackupByFileName(file);
        if (backup is null) {
            return NotFound("Backup not found");
        }

        backupService.RestoreBackup(file);
        return Ok("Backup applied");
    }

    [HttpDelete("/delete-backup")]
    public IActionResult DeleteBackup(string file)
    {
        if (userSession.isAuthenticated is false) {
            return Redirect("/login");
        }

        var backup = backupService.GetBackupByFileName(file);
        if (backup is null) {
            return NotFound("Backup not found");
        }
        
        backupService.DeleteBackup(file);
        return Ok("Backup deleted");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
