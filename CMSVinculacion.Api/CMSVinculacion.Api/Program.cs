using CMSVinculacion.Api.Extensions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

builder.Services
    .DependencyEF(builder.Configuration)
    .AddApplicationServices();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

builder.Services.AddDataProtection();

var app = builder.Build();
app.MapGet("/hash", () => BCrypt.Net.BCrypt.HashPassword("1234"));//clave

var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>))!;
startup.Configure(app, app.Environment, servicioLogger);

app.UseStaticFiles();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();