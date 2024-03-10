using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClienteTCP.Models.Dtos;
using System.Text.Json;

namespace ClienteTCP.Services
{
    public class ComunicacionService
    {
        TcpClient cliente = null!;
        public string Equipo { get; set; } = null!;
        public List<string> Errores { get; set; } = new();
        public bool Conectar(IPAddress ip)
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(ip, 9000);
                cliente = new();
                cliente.Connect(ipe);
                Equipo = Dns.GetHostName();


                var foto = new FotoDto
                {
                    Usuario = Equipo,
                    Estado = "**HELLO",
                    Fecha = DateTime.Now
                };

                EnviarFoto(foto);

                return true;
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
                return false;
            }
        }
        ///CLIENTE 
        public void EnviarFoto(FotoDto foto)
        {
            try
            {
                if (foto != null)
                {
                    //SI LA FOTO NO ESTA VACIA LA CODIFICAMOS A BASE 64
                    if (!string.IsNullOrWhiteSpace(foto.Foto))
                    {
                        var imagencodificada = System.IO.File.ReadAllBytes(foto.Foto);
                        foto.Foto = Convert.ToBase64String(imagencodificada); // CODIFICAMOS LA FOTO
                    }
                    
                    ///CLIENTE 
                    //SERIALIZAMOS EL OBJETO DE FOTO 
                    var json = JsonSerializer.Serialize(foto);

                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    
                    cliente.SendBufferSize = buffer.Length;
                   
                    var ns = cliente.GetStream();

                    ns.Write(buffer, 0, buffer.Length);
                    ns.Flush();
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }


        public void EliminarFoto(FotoDto foto)
        {
            if (!string.IsNullOrWhiteSpace(foto.Id))
            {
                var json = JsonSerializer.Serialize(foto);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                cliente.SendBufferSize = buffer.Length;
                var ns = cliente.GetStream();   
                ns.Write(buffer,0, buffer.Length);
                ns.Flush();
            }
        }


        public void Desconestar()
        {
            var foto = new FotoDto()
            {
                Usuario = Equipo,
                Estado = "**BYE",
                Fecha = DateTime.Now
            };

            EnviarFoto(foto);

            cliente.Close();
        }
        public void Eliminar(FotoDto dto)
        {
            if (dto != null)
            {
                var json=JsonSerializer.Serialize(dto);
                var buffer=Encoding.UTF8.GetBytes(json);
                var ns=cliente.GetStream();
                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
            }
        }
    }
}
