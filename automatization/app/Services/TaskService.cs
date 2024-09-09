namespace Services;

public class TaskService : IHostedService
{
    private List<Models.Task> tasks = new List<Models.Task>();
    private System.Timers.Timer timer;
    private const int ExpirationThresholdHours = 6;

    public TaskService()
    {
        Console.WriteLine("Task service was started");
        timer = new System.Timers.Timer(5000); // Timer to check tasks every 5 seconds
        timer.Elapsed += Timer_Elapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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

    public async System.Threading.Tasks.Task RunCommand(Models.Task task)
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

    public void AddTask(Models.Task task)
    {
        tasks.Add(task);
    }

    public Models.Task GetTaskById(string taskId)
    {
        return tasks.FirstOrDefault(task => task.Uuid == taskId);
    }

    public List<Models.Task> GetAllTasks()
    {
        return tasks.ToList();
    }

    public void DeleteTask(string taskId)
    {
        tasks.RemoveAll(task => task.Uuid == taskId && task.Status != Models.Task.TaskStatus.Processing);
    }

    public void DeleteAll() {
        tasks.RemoveAll(task => task.Status != Models.Task.TaskStatus.Processing);
    }

    public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new System.Timers.Timer(5000); // Timer to check tasks every 5 seconds
        timer.Elapsed += Timer_Elapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
    {
        timer.Stop();
        return System.Threading.Tasks.Task.CompletedTask;
    }
}
