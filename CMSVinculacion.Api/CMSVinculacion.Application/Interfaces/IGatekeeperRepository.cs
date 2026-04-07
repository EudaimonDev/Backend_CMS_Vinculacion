using CMSVinculacion.Domain.Entities.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IGatekeeperRepository
    {
        Task GuardarVisitanteAsync(VisitanteAcceso visitante);
        Task<VisitanteAcceso?> ObtenerPorTokenAsync(string token);
    }
}
