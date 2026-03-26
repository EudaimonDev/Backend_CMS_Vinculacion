using CMSVinculacion.Api.MIddlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CMSVinculacion.Api.Extensions
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public string DataBaseName { get; }

        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // APPLICATION DB CONTEXT
            services.DependencyEF(Configuration);

            /***************/
            /*Culture Info*/
            /***************/
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var esCultureInfo = new CultureInfo("es-EC");
                var enCultureInfo = new CultureInfo("en");
                //var srCultureInfo = new CultureInfo("sr");

                esCultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                esCultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
                esCultureInfo.NumberFormat.PerMilleSymbol = ",";
                esCultureInfo.NumberFormat.CurrencyGroupSeparator = ";";

                var supportedCultures = new[]
                {
                                esCultureInfo,
                                enCultureInfo
                                //srCultureInfo
                 };

                options.DefaultRequestCulture = new RequestCulture(culture: enCultureInfo, uiCulture: esCultureInfo);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                            {
                                new QueryStringRequestCultureProvider(),
                                new CookieRequestCultureProvider()
                            };
            });

            /******************************************************************************/
            /*                                                                             */
            /*                                                                             */
            /*                                 LOGS                                        */
            /*                                                                             */
            /*                                                                             */
            /******************************************************************************/


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
                .WriteTo.File($"logs/Vinculacion-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(Log.Logger);
            });

            // Modificado para token encriptado
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                 AddJwtBearer(opciones =>
                 {
                     opciones.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                         ClockSkew = TimeSpan.Zero
                     };
                     opciones.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = async context =>
                         {
                             var token = context.HttpContext.Items["DecryptedToken"] as string;
                             context.Token = token?[7..];
                             await Task.CompletedTask;
                         },
                     };
                 });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            // CACHE
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vinculacion.Api",
                    Version = "v1",
                    Description = "API RESTful para la gestion de Blog",
                    Contact = new OpenApiContact
                    {
                        Email = "soporte@ug.com",
                        Name = "UG",
                        Url = new Uri("https://apptelink.com")
                    },
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            });

            //Security on login
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 3; // Número de intentos fallidos permitidos antes de bloquear la cuenta
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60); // Duración del bloqueo de cuenta
            });
            services.AddDataProtection();

            //CORS es relevante para navegadores y proyectos hechos en react, angular, etc, para aplicaciones moviles y de mas no tiene sentido realizarlo 
            string[] corsHostClients = Configuration.GetSection("CORSDomainClients").Get<string[]>()
                ?? throw new Exception("Add a client domain for CORS to CORSDomainClients env(array)");


            services.AddCors(options =>
            {
                options.AddPolicy(
                    "origen1",
                    builder =>
                    {
                        builder
                        .WithOrigins(corsHostClients)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    }
                );
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDataProtection();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLoguearRespuestaHTTP();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VinculaciónUG.API v1");
            });

            app.UseCors("origen1");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
