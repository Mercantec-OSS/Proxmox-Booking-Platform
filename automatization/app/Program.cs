
var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddTransient<ICommand, PowerShellCommand>();
builder.Services.AddTransient<ICommand, ShellCommand>();
builder.Services.AddHostedService<TaskBackgoundService>();
builder.Services.AddSingleton<ScriptFactory>();
builder.Services.AddSingleton<Config>();
builder.Services.AddScoped<VmBookingService>();
builder.Services.AddScoped<CLusterBookingService>();
builder.Services.AddHostedService<VCenterInfoBackgroundService>();
builder.Services.AddHostedService<TemplatesBackgroundService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
