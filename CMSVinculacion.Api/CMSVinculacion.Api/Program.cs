using CMSVinculacion.Api.Extensions;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Application.Services;
using CMSVinculacion.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Add services to the container.
builder.Services.AddScoped<IGatekeeperRepository, GatekeeperRepository>();
builder.Services.AddScoped<IGatekeeperService, GatekeeperService>();
// Aumentar límite de tamaño del request (por defecto 30MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});
// También puedes aumentar el límite en Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});
builder.Services.AddDataProtection();
var app = builder.Build();

var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));

startup.Configure(app, app.Environment, servicioLogger);
app.UseStaticFiles();
app.UseWebSockets();

await app.RunAsync();
