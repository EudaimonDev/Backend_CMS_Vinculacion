using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Application.DTOs.gatekeeper
{
    public class GatekeeperRequestDto
    {
        public int Edad { get; set; }
        public string Sexo { get; set; } = string.Empty;

        public Dictionary<string, object>? Extras { get; set; }
    }
}