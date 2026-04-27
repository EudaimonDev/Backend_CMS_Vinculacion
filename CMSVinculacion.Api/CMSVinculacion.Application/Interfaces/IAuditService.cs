using CMSVinculacion.Domain.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(AuditLog log);
    }
}
