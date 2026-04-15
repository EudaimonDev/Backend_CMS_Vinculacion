using CMSVinculacion.Domain.Entities.Catalogos;
using CMSVinculacion.Domain.Entities.Contenido;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using CMSVinculacion.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure
{
    public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
    {
        // CATALOGO
        public DbSet<Categories> Categories { get; set; }

        // CONTENIDO
        public DbSet<Articles> Articles { get; set; }
        public DbSet<ArticleStatus> ArticleStatus { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<MediaFiles> MediaFiles { get; set; }

        // SEGURIDAD
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // GATEKEEPER
        public DbSet<Visitors> Visitors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // varchar por defecto
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                    if (property.ClrType == typeof(string))
                    {
                        var maxLength = property.GetMaxLength();
                        property.SetColumnType(maxLength != null ? $"varchar({maxLength})" : "varchar(max)");
                    }

            // Articles -> Author
            modelBuilder.Entity<Articles>()
                .HasOne(a => a.Author)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Articles -> Status
            modelBuilder.Entity<Articles>()
                .HasOne(a => a.Status)
                .WithMany(s => s.Articles)
                .HasForeignKey(a => a.StatusId);

            // Articles <-> Categories (N:M via ArticleCategory)
            modelBuilder.Entity<ArticleCategory>()
                .HasKey(ac => new { ac.ArticleId, ac.CategoryId });

            modelBuilder.Entity<ArticleCategory>()
                .HasOne(ac => ac.Article)
                .WithMany(a => a.ArticleCategories)
                .HasForeignKey(ac => ac.ArticleId);

            modelBuilder.Entity<ArticleCategory>()
                .HasOne(ac => ac.Category)
                .WithMany(c => c.ArticleCategories)
                .HasForeignKey(ac => ac.CategoryId);

            // MediaFiles -> Article
            modelBuilder.Entity<MediaFiles>()
                .HasOne(m => m.Article)
                .WithMany(a => a.MediaFiles)
                .HasForeignKey(m => m.ArticleId);

            // Users -> Role
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // AuditLog -> User
            modelBuilder.Entity<AuditLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);
        }
    }
}