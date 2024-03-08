using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClienteTCP.Models.Dtos;

namespace ClienteTCP.Services
{
    public class ComunicacionService
    {
        TcpClient cliente = null!;
        public string equipo { get; set; } = null!;
        public void Conectar(IPAddress ip)
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(ip, 9000);
                cliente = new();
                cliente.Connect(ipe);
                equipo = Dns.GetHostName();
                var msg = new FotoDto
                {
                    UserName = equipo,
                };
               
                
            }
            catch (Exception ex)
            {
                //Mostrar el error.
            }
        }
    }
}
