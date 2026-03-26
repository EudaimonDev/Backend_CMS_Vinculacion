namespace CMSVinculacion.Api.Extensions
{
    public static class DependencyInyection
    {
        public static IServiceCollection DependencyEF(this IServiceCollection services,
       IConfiguration configuration)
        {
           // services.AddScoped<IUserRepository, UserRepository>();
           return services;
        }
    }
}
