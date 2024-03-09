using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTCP.Models.Dtos
{
    public class FotoDto
    {
        public string Id { get; set; } = null!;
        public string Foto { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime Fecha { get; set; }

    }
}
