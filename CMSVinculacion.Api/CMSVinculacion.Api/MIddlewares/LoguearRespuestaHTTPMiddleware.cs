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
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
        {
            this.siguiente = siguiente;
            dataProtector = dataProtectionProvider.CreateProtector(configuration["LlaveProtector"]);
        }

        public async Task InvokeAsync(HttpContext contexto, IAuditLogRepository auditRepo)
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;

            try
            {
                using var ms = new MemoryStream();
                contexto.Response.Body = ms;

                string authorizationHeader = contexto.Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(authorizationHeader) &&
                    authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string tokenLimpio = authorizationHeader["Bearer ".Length..].Trim();
                        string desencriptar = Desencriptar(tokenLimpio);
                        contexto.Items["DecryptedToken"] = desencriptar;
                    }
                    catch
                    {
                        // no rompe la API por token
                    }
                }
                await siguiente(contexto);
                foreach (var c in contexto.User.Claims)
                {
                    Console.WriteLine($"{c.Type} -> {c.Value}");
                }
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

                var log = new AuditLog
                {
                    Action = contexto.Request.Method,
                    Entity = contexto.Request.Path,
                    IPAddress = contexto.Connection.RemoteIpAddress?.ToString(),
                    Detail = $"StatusCode: {contexto.Response.StatusCode}",
                    UserId = userId
                };
                try
                {
                    await auditRepo.AddAsync(log);
                }
                catch
                {
                    // nunca romper la API por logging
                }
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;
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