// public class VCenterInfoBackgroundService : BackgroundService
// {
//     private static VCenterInfoDTO _data = new VCenterInfoDTO();
//     VmBookingScriptService vmBookingService = new VmBookingScriptService(new ScriptService());

//     public static VCenterInfoDTO GetInfo()
//     {
//         return _data;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // Special case: for faster app starting
//         await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 _data = VCenterInfoDTO.FromCommandOutput(vmBookingService.GetVcenterInfo());
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }

//             await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
//         }
//     }
// }