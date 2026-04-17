using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
