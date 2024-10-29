public class TemplatesBackgroundService : BackgroundService
{
    private static List<string> _data = new List<string>();
    VmBookingService vmBookingService = new VmBookingService(new ScriptFactory(new Config()));

    public static List<string> GetTemplates()
    {
        return _data;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Special case: for faster app starting
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _data = vmBookingService.GetTemplates();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}