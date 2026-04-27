using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlDbContext _context;

        public UserRepository(SqlDbContext context) => _context = context;

        public async Task<Users?> GetByEmailAsync(string email) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        public async Task<Users?> GetByIdAsync(int id) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

        public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            user.LastLogin = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<Users?> GetByRefreshTokenAsync(string refreshToken) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);
    }
}