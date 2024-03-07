using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServidorTCP.Services
{
    public class FotosService
    {
        TcpListener server = null!;
        List<TcpClient> clientes = new List<TcpClient>();
        

        public void Iniciar()
        {
            server = new(new IPEndPoint(IPAddress.Any, 9000));
            server.Start();
            new Thread(Escuchar) { IsBackground = true }.Start();
        }

        private void Escuchar(object? obj)
        {
            while(server.Server.IsBound)
            {
                var tcpClient = server.AcceptTcpClient();
                clientes.Add(tcpClient);

                new Thread(() =>
                {
                    RecibirMensajes(tcpClient);
                })
                { IsBackground = true};

            }
        }

        private void RecibirMensajes(TcpClient tcpClient)
        {
            throw new NotImplementedException();
        }
    }
}
