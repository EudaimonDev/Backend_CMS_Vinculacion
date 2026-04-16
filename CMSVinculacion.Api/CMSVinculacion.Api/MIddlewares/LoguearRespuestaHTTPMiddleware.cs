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
                //string token = contexto.Request.Headers["Authorization"];
                //if (token != null && !token.Contains("Basic"))
                //{
                //    string desencriptar = Desencriptar(token);
                //    contexto.Items["DecryptedToken"] = desencriptar;
                //    string resultado = contexto.Items["DecryptedToken"] as string;
                //}
                //var cuerpoOriginalRespuesta = contexto.Response.Body;
                //contexto.Response.Body = ms;

                //// aqui se envia 
                //await siguiente(contexto);

                //ms.Seek(0, SeekOrigin.Begin);
                //string respuesta = new StreamReader(ms).ReadToEnd();
                //ms.Seek(0, SeekOrigin.Begin);

                //await ms.CopyToAsync(cuerpoOriginalRespuesta);
                //contexto.Response.Body = cuerpoOriginalRespuesta;

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
