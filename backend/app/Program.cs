WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ScriptService>();
builder.Services.AddScoped<ScriptFactory>();
builder.Services.AddScoped<VmBookingService>();
builder.Services.AddScoped<VmBookingScriptService>();
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<Config>();
builder.Services.AddHostedService<SchedulerBackgroundService>();
builder.Services.AddHostedService<VCenterInfoBackgroundService>();
builder.Services.AddHostedService<TemplatesBackgroundService>();
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddSwaggerGen();


// Db initialization
builder.Services.AddDbContext<Context>(options =>
{
    string? connString = builder.Configuration.GetConnectionString("DB_CONNECTION_STRING");

    if (string.IsNullOrEmpty(connString))
    {
        connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    }

    if (string.IsNullOrEmpty(connString))
    {
        throw new Exception("Connection string not found");
    }

    ServerVersion serverVersion = ServerVersion.Parse(connString);
    options.UseMySql(connString, serverVersion);
});

WebApplication app = builder.Build();

// Extra functionality under development
if (app.Environment.IsDevelopment())
{
    Context.MakeMigration(app);

    app.UseCors("AllowAll");

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Extra functionality under production
if (app.Environment.IsProduction())
{
    if (Environment.GetEnvironmentVariable("ALLOW_CORS") == "true")
    {
        app.UseCors("AllowAll");
    }

    if (Environment.GetEnvironmentVariable("USE_SWAGGER") == "true")
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (Environment.GetEnvironmentVariable("AUTO_MIGRATIONS") == "true")
    {
        Context.MakeMigration(app);
    }
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles("/web-console/src");

app.Run();
