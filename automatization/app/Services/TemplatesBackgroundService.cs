public class TemplatesBackgroundService : BackgroundService
{
    private static List<string> _data = new List<string>();
    private static DateTime? lastUpdate = null;
    VmBookingService vmBookingService = new VmBookingService(new ScriptFactory(new Config()));

    public static List<string> GetTemplates()
    {
        return _data;
    }

    public static void ResetTemplates()
    {
        _data = new List<string>();
        lastUpdate = null;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Special case: for faster app starting
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Update data every 5 minutes
            if (lastUpdate == null || DateTime.UtcNow - lastUpdate > TimeSpan.FromMinutes(5))
            {
                try
                {
                    _data = vmBookingService.GetTemplates();
                    lastUpdate = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}