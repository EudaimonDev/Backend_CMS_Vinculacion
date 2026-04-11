using CMSVinculacion.Domain.Entities.Catalogos;
using CMSVinculacion.Domain.Entities.Contenido;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using CMSVinculacion.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure
{
    public class SqlDbContext(
         DbContextOptions<SqlDbContext> options) : DbContext(options)
    {
        //CATALOGO
        public DbSet<Categories> Categories { get; set; }

        //CONTENIDO
        public DbSet<Articles> Articles { get; set; }
        public DbSet<ArticleStatus> ArticleStatus { get; set; }
        public DbSet<MediaFiles> MediaFiles { get; set; }

        //SEGURIDAD
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        //GATEKEEPER
        public DbSet<Visitors> Visitors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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
            modelBuilder.Entity<Articles>()
               .HasOne(a => a.Author)
               .WithMany(u => u.Articles)
               .HasForeignKey(a => a.AuthorId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Articles>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId);

            modelBuilder.Entity<Articles>()
                .HasOne(a => a.Status)
                .WithMany(s => s.Articles)
                .HasForeignKey(a => a.StatusId);

            modelBuilder.Entity<MediaFiles>()
                .HasOne(m => m.Article)
                .WithMany(a => a.MediaFiles)
                .HasForeignKey(m => m.ArticleId);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<AuditLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);
        }

        //crear la funcion de aplicacion de auditoria automatica
    }
}
