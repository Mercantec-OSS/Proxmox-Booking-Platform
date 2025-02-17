public class ClusterInfoBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private static ClusterInfoDto _data = new ClusterInfoDto();

    public static ClusterInfoDto GetInfo()
    {
        return _data;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Special case: for faster app starting
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var vmService = scope.ServiceProvider.GetRequiredService<VmService>();

            try
            {
                _data = await vmService.GetClusterInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}