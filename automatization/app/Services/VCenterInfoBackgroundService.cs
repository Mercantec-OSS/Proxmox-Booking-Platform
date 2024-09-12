public class VCenterInfoBackgroundService : BackgroundService
{
    private static VCenterInfoDTO _data = new VCenterInfoDTO();
    VmBookingService vmBookingService = new VmBookingService(new ScriptFactory(new Config()));

    public VCenterInfoDTO GetInfo()
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
                _data = VCenterInfoDTO.FromCommandOutput(vmBookingService.GetVcenterInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}