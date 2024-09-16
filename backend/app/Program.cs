using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ScriptService>();
builder.Services.AddScoped<LdapService>();
builder.Services.AddScoped<ClusterBookingService>();
builder.Services.AddScoped<VmBookingService>();
builder.Services.AddScoped<EsxiHostService>();
builder.Services.AddScoped<VCenterService>();
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddSingleton<ActivityService>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<Services.AutoServices>();
builder.Services.AddSingleton<Config>();
builder.Services.AddHostedService<ActivityService>();
builder.Services.AddHostedService<Services.AutoServices>();
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

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
                new List<string>()
        }
    });
});


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

Services.AutoServices autoServices = new();
autoServices.SetDB(app);

app.Run();
