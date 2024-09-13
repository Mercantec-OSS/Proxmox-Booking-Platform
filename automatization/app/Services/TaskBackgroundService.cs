namespace Services;

public class TaskBackgoundService : BackgroundService
{
    private static List<Models.Task> tasks = new List<Models.Task>();
    private const int ExpirationThresholdHours = 6;


    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Special case: for faster app starting
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            DoTask();
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private void DoTask()
    {
        foreach (var task in tasks.ToList())
        {
            // Check if the task is waiting and has exceeded the expiration time
            if (task.Status == Models.Task.TaskStatus.Complete && DateTime.Now - task.CreatedAt > TimeSpan.FromHours(ExpirationThresholdHours))
            {
                tasks.Remove(task);
                continue; 
            }

            if (task.Status == Models.Task.TaskStatus.Waiting)
            {
                // Check if the task should start immediately or after another task
                if (string.IsNullOrEmpty(task.AfterThan))
                {
                    _ = RunCommand(task);
                }
                else
                {
                    var dependentTask = tasks.FirstOrDefault(t => t.Uuid == task.AfterThan);
                    if (dependentTask != null && dependentTask.Status == Models.Task.TaskStatus.Complete)
                    {
                        _ = RunCommand(task);
                    }
                    else if (dependentTask == null)
                    {
                        tasks.Remove(task);
                    }
                }
            }
        }
    }

    public static async System.Threading.Tasks.Task RunCommand(Models.Task task)
    {
        task.Status = Models.Task.TaskStatus.Processing;
        task.StartTime = DateTime.UtcNow; 
        try
        {
            task.Output = await System.Threading.Tasks.Task.Run(() => task.Command.Execute(true));
            task.Status = Models.Task.TaskStatus.Complete;
        }
        catch (Exception ex)
        {
            task.Output = $"Error occurred: {ex.Message}"; 
            task.Status = Models.Task.TaskStatus.Complete; 
        }
        finally
        {
            task.EndTime = DateTime.UtcNow; 
        }
    }

    public static void AddTask(Models.Task task)
    {
        tasks.Add(task);
    }

    public static Models.Task? GetTaskById(string taskId)
    {
        return tasks.FirstOrDefault(task => task.Uuid == taskId);
    }

    public static List<Models.Task> GetAllTasks()
    {
        return tasks.ToList();
    }

    public static void DeleteTask(string taskId)
    {
        tasks.RemoveAll(task => task.Uuid == taskId && task.Status != Models.Task.TaskStatus.Processing);
    }

    public static void DeleteAll() {
        tasks.RemoveAll(task => task.Status != Models.Task.TaskStatus.Processing);
    }
}
