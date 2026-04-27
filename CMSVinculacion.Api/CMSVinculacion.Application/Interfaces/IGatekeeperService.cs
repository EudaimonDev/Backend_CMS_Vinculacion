using CMSVinculacion.Application.DTOs.gatekeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IGatekeeperService
    {
        Task<GatekeeperResponseDto> RegistrarVisitanteAsync(
        GatekeeperRequestDto request, string ipAddress);

        Task<GatekeeperResponseDto> ValidarTokenAsync(string token);
    }
}
