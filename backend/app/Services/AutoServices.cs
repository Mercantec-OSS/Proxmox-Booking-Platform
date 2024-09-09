namespace Services;

public class AutoServices : IHostedService
{
    private readonly System.Timers.Timer RunAutoJobs = new()
    {
        AutoReset = true,
        Enabled = false
    };

    private readonly List<AutoService> services =
    [
        new AutoSyncDBWithAD()
    ];

    private static WebApplication app = null!;
    public void SetDB(WebApplication app) => AutoServices.app = app;

    private async void RunAutoTasks(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Config config = new Config();
        CalculateTimerInterval();
        foreach (AutoService autoService in services)
            await autoService.Start(app, config);
    }

    private void CalculateTimerInterval()
    {
        DateTime now = DateTime.Now;
        DateTime scheduledTime = new(now.Year, now.Month, now.Day, 2, 0, 0, DateTimeKind.Utc);

        if (now > scheduledTime)
            scheduledTime = scheduledTime.AddDays(1);

        TimeSpan timeToGo = scheduledTime - now;
        RunAutoJobs.Interval = timeToGo.TotalMilliseconds;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        CalculateTimerInterval();
        RunAutoJobs.Elapsed += RunAutoTasks;
        RunAutoJobs.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        RunAutoJobs.Stop();
        return Task.CompletedTask;
    }
}
