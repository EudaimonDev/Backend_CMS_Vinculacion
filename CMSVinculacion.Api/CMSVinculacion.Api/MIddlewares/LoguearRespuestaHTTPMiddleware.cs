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
                string token = contexto.Request.Headers["Authorization"];
               if (token != null && !token.Contains("Basic"))
                {
                    string desencriptar = Desencriptar(token);
                    contexto.Items["DecryptedToken"] = desencriptar;
                    string resultado = contexto.Items["DecryptedToken"] as string;
                }
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                // aqui se envia 
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
