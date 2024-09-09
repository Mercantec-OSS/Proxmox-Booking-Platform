namespace AutoServices;

public abstract class AutoService
{
    public async Task Start(WebApplication app, Config config)
    {
        using IServiceScope scopeDb = app.Services.CreateScope();
        Context service = scopeDb.ServiceProvider.GetRequiredService<Context>();
        await StartAuto(service, config);
    }

    protected abstract Task StartAuto(Context service, Config config);
}
