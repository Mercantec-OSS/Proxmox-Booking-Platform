﻿namespace Controllers;

[ApiController]
[Route("other")]
public class OtherController(Config config) : ControllerBase
{

    [HttpGet("run-shell")]
    public async Task<IActionResult> RunShell([FromQuery] string command)
    {
        var shellCommand = new ShellCommand(command);
        var task = new Models.Task { Command = shellCommand };

        await TaskBackgoundService.RunCommand(task);
        return Ok(task.Output);
    }

    [HttpGet("run-powershell")]
    public async Task<IActionResult> RunPowerShell([FromQuery] string command)
    {
        var pwshCommand = new PowerShellCommand(command);
        var task = new Models.Task { Command = pwshCommand };

        await TaskBackgoundService.RunCommand(task);
        return Ok(task.Output);
    }

    [HttpGet("/version")]
    public IActionResult GetVersion()
    {
        return Ok(config.VERSION);
    }
}
