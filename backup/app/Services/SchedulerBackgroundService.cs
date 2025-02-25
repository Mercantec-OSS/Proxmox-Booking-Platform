public class SchedulerBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunTasks();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }

    private async Task RunTasks()
    {
        var now = DateTime.UtcNow;

        // at 02:00 UTC every day
        if (now.Hour == 2 && now.Minute == 0)
        {
            try
            {
                AutoCreateBackup();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to create backup");
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private void AutoCreateBackup()
    {
        using var scope = scopeFactory.CreateScope();
        var backupService = scope.ServiceProvider.GetRequiredService<BackupService>();

        backupService.CreateBackup();
    }
}
