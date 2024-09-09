namespace Controllers;

[Route("task")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskService taskService;

    public TaskController(TaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CommandRequestModel requestModel)
    {
        if (requestModel == null || requestModel.Commands == null || requestModel.Commands.Count == 0)
        {
            return BadRequest("No commands provided");
        }

        foreach (var command in requestModel.Commands)
        {
            var task = new Models.Task();
            task.Command = GetCommandInstance(command);
            taskService.AddTask(task);
        }

        return Ok("Tasks created successfully");
    }

    private ICommand GetCommandInstance(CommandModel command)
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
        var task = taskService.GetTaskById(taskId);
        if (task != null)
            return Ok(task);
        else
            return NotFound("Task not found");
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var tasks = taskService.GetAllTasks();
        return Ok(tasks);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(204)]
    public IActionResult Delete(string taskId)
    {
        taskService.DeleteTask(taskId);
        return NoContent();

    }

    [HttpDelete("delete-all")]
    [ProducesResponseType(204)]
    public IActionResult DeleteAll()
    {
        taskService.DeleteAll();
        return NoContent();
    }
}
