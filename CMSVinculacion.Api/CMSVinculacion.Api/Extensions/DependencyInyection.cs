using CMSVinculacion.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Api.Extensions
{
    public static class DependencyInyection
    {
        public static IServiceCollection DependencyEF(this IServiceCollection services,
       IConfiguration configuration)
        {
            // services.AddScoped<IUserRepository, UserRepository>();

            var connectionString = configuration.GetConnectionString("defaultConnection");


            return services.AddDbContext<SqlDbContext>(options =>
                options.UseSqlServer(connectionString)
             );
        }
    }
}
