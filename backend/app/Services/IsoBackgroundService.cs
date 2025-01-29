// public class IsoBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
// {
//     private static List<Iso> _data = new List<Iso>();
//     private static DateTime? lastUpdate = null;

//     public static List<string> GetAllNames()
//     {
//         return _data.Select(iso => iso.Name).ToList();
//     }

//     public static Iso? GetByName(string name)
//     {
//         return _data.FirstOrDefault(iso => iso.Name == name);
//     }

//     public static void Reset()
//     {
//         _data.Clear();
//         lastUpdate = null;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // Special case: for faster app starting
//         await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

//         while (!stoppingToken.IsCancellationRequested)
//         {
//             using var scope = scopeFactory.CreateScope();
//             VmBookingScriptService vmBookingScriptService = scope.ServiceProvider.GetRequiredService<VmBookingScriptService>();

//             // Update data every 5 minutes
//             if (lastUpdate == null || DateTime.UtcNow - lastUpdate > TimeSpan.FromMinutes(5))
//             {
//                 try
//                 {
//                     _data = vmBookingScriptService.GetIsoList();
//                     lastUpdate = DateTime.UtcNow;
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine(ex.Message);
//                 }
//             }

//             await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
//         }
//     }
// }