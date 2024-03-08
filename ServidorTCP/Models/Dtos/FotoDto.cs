using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorTCP.Models.Dtos
{
    public class FotoDto
    {
        public string Foto { get; set; } = null!;
        public string Origen { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}
