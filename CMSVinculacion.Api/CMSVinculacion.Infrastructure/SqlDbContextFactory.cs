using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CMSVinculacion.Infrastructure
{
    public class SqlDbContextFactory : IDesignTimeDbContextFactory<SqlDbContext>
    {
        public SqlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS01;Database=CMSVinculacion;Trusted_Connection=True;TrustServerCertificate=True");

            return new SqlDbContext(optionsBuilder.Options);
        }
    }
}