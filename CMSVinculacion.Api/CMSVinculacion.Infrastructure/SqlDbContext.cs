using CMSVinculacion.Domain.Entities.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure
{
    public class SqlDbContext(
         DbContextOptions<SqlDbContext> options) : DbContext(options)
    {
        public DbSet<Persona> Personas => Set<Persona>();
        public DbSet<VisitanteAcceso> VisitantesAcceso { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configuración general para tipos string
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        // Verifica si ya tiene una longitud definida
                        var maxLength = property.GetMaxLength();
                        property.SetColumnType(maxLength != null ? $"varchar({maxLength})" : "varchar(max)");
                    }
                }
            }
        }

        //crear la funcion de aplicacion de auditoria automatica
    }
}
