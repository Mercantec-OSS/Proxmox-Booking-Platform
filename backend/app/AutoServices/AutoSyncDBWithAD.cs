namespace AutoServices;

public class AutoSyncDBWithAD : AutoService
{
    protected override async Task StartAuto(Context service, Config config)
    {
        // await SyncADWithDB.SyncClasses(new(service), new(service));
    }
}
