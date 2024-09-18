namespace Services;

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
            await DeleteExpiredVmBookings();
            await DeleteExpiredClusterBookings();
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private async Task DeleteExpiredVmBookings()
    {
        using var scope = scopeFactory.CreateScope();
        var vmService = scope.ServiceProvider.GetRequiredService<VmBookingService>();
        var scriptService = scope.ServiceProvider.GetRequiredService<ScriptService>();

        var expiredBookings = await vmService.GetExpiredAsync();

        foreach (var booking in expiredBookings)
        {
            await vmService.DeleteAsync(booking);
            await scriptService.DeleteVmAsync(booking.Name);
        }
    }

    private async Task DeleteExpiredClusterBookings()
    {
        using var scope = scopeFactory.CreateScope();
        var clusterService = scope.ServiceProvider.GetRequiredService<ClusterBookingService>();
        var scriptService = scope.ServiceProvider.GetRequiredService<ScriptService>();

        var expiredBookings = await clusterService.GetExpiredAsync();

        foreach (var booking in expiredBookings)
        {
            await clusterService.DeleteAsync(booking);
            await scriptService.ResetClusterBookingAsync(booking.VCenters);
        }
    }
}
