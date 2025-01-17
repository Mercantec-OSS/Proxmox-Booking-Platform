public class InviteKeysService() : BackgroundService
{
    private static string AdminInviteKey = "";
    private static string TeacherInviteKey = "";
    private static DateTime? lastUpdate = null;

    public static string GetAdminInviteKey()
    {
        return AdminInviteKey;
    }

    public static string GetTeacherInviteKey()
    {
        return TeacherInviteKey;
    }

    private static void UpdateKeys()
    {
        lastUpdate = DateTime.UtcNow;
        AdminInviteKey = Guid.NewGuid().ToString();
        TeacherInviteKey = Guid.NewGuid().ToString();

        Console.WriteLine("AdminInviteKey: " + AdminInviteKey);
        Console.WriteLine("TeacherInviteKey: " + TeacherInviteKey);
        Console.WriteLine("Invite keys updated: " + lastUpdate);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        UpdateKeys();
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.UtcNow;

            // at 02:00 UTC every day
            if (now.Hour == 2 && now.Minute == 0)
            {
                UpdateKeys();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}