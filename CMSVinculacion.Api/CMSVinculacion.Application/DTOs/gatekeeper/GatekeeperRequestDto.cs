using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.DTOs.gatekeeper
{
    public class GatekeeperRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
    }
}
