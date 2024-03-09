using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorTCP.Models.Dtos
{
    public class FotoDto
    {
        public string Id { get; set; } = null!;
        public string Foto { get; set; } = null!;
        public string Usuario { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime Fecha { get; set; }
    }
}
