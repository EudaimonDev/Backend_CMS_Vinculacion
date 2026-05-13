using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Seguridad;
using CMSVinculacion.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMSVinculacion.Api.MIddlewares
{
    public static class LoguearRespuestaHTTPMiddlewareExtentions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }

    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly IDataProtector dataProtector;

        public LoguearRespuestaHTTPMiddleware(
            RequestDelegate siguiente,
            ILogger<LoguearRespuestaHTTPMiddleware> logger,
            IConfiguration configuration)
        {
            this.siguiente = siguiente;
        }

        public async Task InvokeAsync(HttpContext contexto, IAuditLogRepository auditRepo)
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;
            using var ms = new MemoryStream();

            try
            {
                contexto.Response.Body = ms;
                string requestBody = string.Empty;
                if (contexto.Request.Method is "POST" or "PUT" or "PATCH")
                {
                    contexto.Request.EnableBuffering();
                    using var reader = new System.IO.StreamReader(
                        contexto.Request.Body,
                        leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    contexto.Request.Body.Position = 0;
                }
                await siguiente(contexto);

                // Leer email directo del claim JWT (ya procesado por el middleware de auth)
                string email =
                    contexto.User.FindFirst(ClaimTypes.Email)?.Value
                    ?? contexto.User.FindFirst("email")?.Value;

                int? userId = null;
                if (!string.IsNullOrEmpty(email))
                {
                    using var scope = contexto.RequestServices.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
                    userId = await db.Users
                        .Where(u => u.Email == email)
                        .Select(u => (int?)u.UserId)
                        .FirstOrDefaultAsync();
                }
                string entityName = string.Empty;
                if (!string.IsNullOrEmpty(requestBody))
                {
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(requestBody);
                        entityName = json.RootElement.TryGetProperty("title", out var title)
                            ? title.GetString() ?? string.Empty
                            : json.RootElement.TryGetProperty("name", out var name)
                                ? name.GetString() ?? string.Empty
                                : string.Empty;
                    }
                    catch { }
                }
                var log = new AuditLog
                {
                    Action = contexto.Request.Method,
                    Entity = contexto.Request.Path,
                    IPAddress = contexto.Connection.RemoteIpAddress?.ToString(),
                    Detail = !string.IsNullOrEmpty(entityName)
                        ? entityName
                        : $"StatusCode: {contexto.Response.StatusCode}",
                    UserId = userId
                };

                try { await auditRepo.AddAsync(log); } catch { }

                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuditMiddleware] Error: {ex.Message}");
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
            }
            finally
            {
                contexto.Response.Body = cuerpoOriginalRespuesta;
            }
        }

        private string Desencriptar(string tokenEncriptado)
        {
            return dataProtector.Unprotect(tokenEncriptado);
        }
    }
}