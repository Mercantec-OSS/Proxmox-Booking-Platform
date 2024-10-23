namespace Controllers;

[Route("task")]
[ApiController]
public class TaskController : ControllerBase
{
    [HttpPost("create")]
    public IActionResult Create([FromBody] CommandModelDTO command)
    {
        if (command == null)
        {
            return BadRequest("No commands provided");
        }

        var task = new Models.Task();
        task.Command = GetCommandInstance(command);
        task.AfterThan = command.AfterThan;
        TaskBackgoundService.AddTask(task);

        return Ok(task.Uuid);
    }

    private ICommand GetCommandInstance(CommandModelDTO command)
    {
        return command.Type.ToLower() switch
        {
            "shell" => new ShellCommand(command.Command),
            "powershell" => new PowerShellCommand(command.Command),
            _ => throw new ArgumentException("Invalid command type"),
        };
    }

    [HttpGet("get")]
    public IActionResult GetById(string taskId)
    {
        var task = TaskBackgoundService.GetTaskById(taskId);
        if (task != null)
            return Ok(task);
        else
            return NotFound("Task not found");
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var tasks = TaskBackgoundService.GetAllTasks();
        return Ok(tasks);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(204)]
    public IActionResult Delete(string taskId)
    {
        TaskBackgoundService.DeleteTask(taskId);
        return NoContent();

    }

    [HttpDelete("delete-all")]
    [ProducesResponseType(204)]
    public IActionResult DeleteAll()
    {
        TaskBackgoundService.DeleteAll();
        return NoContent();
    }
}
