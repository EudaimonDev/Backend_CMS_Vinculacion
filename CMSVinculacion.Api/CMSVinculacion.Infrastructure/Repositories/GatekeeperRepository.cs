using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class GatekeeperRepository : IGatekeeperRepository
    {
        private readonly SqlDbContext _context;

        public GatekeeperRepository(SqlDbContext context)
        {
            _context = context;
        }

        public async Task GuardarVisitanteAsync(VisitanteAcceso visitante)
        {
            await _context.VisitantesAcceso.AddAsync(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task<VisitanteAcceso?> ObtenerPorTokenAsync(string token)
        {
            return await _context.VisitantesAcceso
                .FirstOrDefaultAsync(v => v.Token == token && v.Active);
        }
    }
}
