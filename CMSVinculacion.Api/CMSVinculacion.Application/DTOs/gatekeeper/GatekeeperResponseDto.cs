using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.DTOs.gatekeeper
{
    public class GatekeeperResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? Expiracion { get; set; }
    }
}
