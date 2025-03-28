﻿namespace Services;

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
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private async Task DeleteExpiredVmBookings()
    {
        using var scope = scopeFactory.CreateScope();
        var vmRepository = scope.ServiceProvider.GetRequiredService<VmBookingRepository>();
        var vmService = scope.ServiceProvider.GetRequiredService<VmService>();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
        var expiredBookings = await vmRepository.GetExpiredAsync();

        foreach (var booking in expiredBookings)
        {
            EmailDto email = EmailDto.GetVmBookingExpired(booking);
            await emailService.SendAsync(email);

            await vmRepository.DeleteAsync(booking);
            _ = vmService.Remove(booking.Name);
        }
    }
}
