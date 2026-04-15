using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class GatekeeperRepository : IGatekeeperRepository
    {
        private readonly SqlDbContext _context;

        public GatekeeperRepository(SqlDbContext context) => _context = context;

        public async Task GuardarVisitanteAsync(Visitors visitante)
        {
            await _context.Visitors.AddAsync(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task<Visitors?> ObtenerPorTokenAsync(string token)
        {
            return await _context.Visitors
                .FirstOrDefaultAsync(v => v.CookieToken == token && v.IsActive);
        }
    }
}