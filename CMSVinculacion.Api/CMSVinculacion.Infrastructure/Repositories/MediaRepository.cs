using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Contenido;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly SqlDbContext _context;

        public MediaRepository(SqlDbContext context) => _context = context;

        public async Task<MediaFiles?> GetByIdAsync(int id) =>
            await _context.MediaFiles.FindAsync(id);

        public async Task<IEnumerable<MediaFiles>> GetAllAsync(int page = 1, int pageSize = 20) =>
            await _context.MediaFiles
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<MediaFiles> CreateAsync(MediaFiles media)
        {
            await _context.MediaFiles.AddAsync(media);
            await _context.SaveChangesAsync();
            return media;
        }

        public async Task DeleteAsync(int id)
        {
            var media = await _context.MediaFiles.FindAsync(id);
            if (media is null) return;
            media.IsActive = false;
            media.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}