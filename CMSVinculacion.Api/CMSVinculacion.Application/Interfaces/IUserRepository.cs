using CMSVinculacion.Domain.Entities.Seguridad;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetByEmailAsync(string email);
        Task<Users?> GetByIdAsync(int id);
        Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
        Task<Users?> GetByRefreshTokenAsync(string refreshToken);
    }
}