using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SqlDbContext _context;

        public AuditLogRepository(SqlDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.Set<AuditLog>().AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAsync(int count = 10)
        {
            return await _context.Set<AuditLog>()
                .Where(a => a.IsActive && a.Action != "GET")  // ← filtrar GETs aquí también
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .Include(a => a.User)
                .ToListAsync();
        }
    }
}
