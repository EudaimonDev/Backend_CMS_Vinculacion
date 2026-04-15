using CMSVinculacion.Domain.Entities.Gatekeeper;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IGatekeeperRepository
    {
        Task GuardarVisitanteAsync(Visitors visitante);
        Task<Visitors?> ObtenerPorTokenAsync(string token);
    }
}