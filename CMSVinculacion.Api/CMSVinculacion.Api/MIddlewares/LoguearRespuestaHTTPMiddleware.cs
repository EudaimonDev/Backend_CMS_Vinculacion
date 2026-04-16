using Microsoft.AspNetCore.DataProtection;

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

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger,
            IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            this.siguiente = siguiente;
            dataProtector = dataProtectionProvider.CreateProtector(configuration["LlaveProtector"]);
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                string authorizationHeader = contexto.Request.Headers["Authorization"];

                // 1. Verificamos que el header exista y sea un esquema que queremos procesar
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        // 2. Extraemos solo el token (quitamos "Bearer ")
                        string tokenLimpio = authorizationHeader.Substring("Bearer ".Length).Trim();

                        string desencriptar = Desencriptar(tokenLimpio);
                        contexto.Items["DecryptedToken"] = desencriptar;
                    }
                    catch (Exception ex)
                    {
                        // Loguea el error pero permite que la petición siga, 
                        // o maneja el error de seguridad según tu lógica.
                        // _logger.LogWarning("No se pudo desencriptar el token: {Message}", ex.Message);
                    }
                }

                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguiente(contexto);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;



            }
        }
        private string Desencriptar(string tokenEncriptado)
        {
            return dataProtector.Unprotect(tokenEncriptado);
        }
    }
}
