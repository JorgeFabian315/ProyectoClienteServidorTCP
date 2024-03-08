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
        public void EnviarFoto(FotoDto foto)
        {
            try
            {
                byte[] imageData = Convert.FromBase64String(foto.Base64EncodedImage);

                // Crear un stream de red para enviar datos al servidor
                var ns = cliente.GetStream();

                // Enviar el tamaño de los datos de la imagen al servidor
                byte[] dataSize = BitConverter.GetBytes(imageData.Length);
                ns.Write(dataSize, 0, dataSize.Length);

                // Enviar los datos de la imagen al servidor
                ns.Write(imageData, 0, imageData.Length);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
