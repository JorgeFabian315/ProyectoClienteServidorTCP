using ServidorTCP.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ServidorTCP.Services
{
    public class FotosService
    {
        TcpListener server = null!;
        List<TcpClient> clientes = new();
        public event EventHandler<FotoDto>? RecibirFotoEvent;
        public List<string> Errores { get; set; } = new();

        public void Iniciar()
        {
            server = new(new IPEndPoint(IPAddress.Any, 9000));
            server.Start();
            new Thread(Escuchar) { IsBackground = true }.Start();
        }
        private void Escuchar(object? obj)
        {
            try
            {
                while (server.Server.IsBound)
                {
                    var tcpClient = server.AcceptTcpClient();
                    clientes.Add(tcpClient);
                    tcpClient.ReceiveBufferSize = 5000000;
                    new Thread(() =>
                    {
                        RecibirFotos(tcpClient);
                    })
                    { IsBackground = true }.Start();
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }

        private void RecibirFotos(TcpClient cliente)
        {
            while (cliente.Connected)
            {
                try
                {

                    var ns = cliente.GetStream();

                    while (cliente.Available == 0)
                    {
                        Thread.Sleep(500);
                    }

                    byte[] buffer = new byte[cliente.Available];

                    ns.Read(buffer, 0, buffer.Length);

                    string json = Encoding.UTF8.GetString(buffer);

                    var foto = JsonSerializer.Deserialize<FotoDto>(json);

                    if (foto != null)
                    {
                        //RelayMensaje(cliente, buffer);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            RecibirFotoEvent?.Invoke(this, foto);
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // Removemos el cliente de la lista de clientes después de que se desconecta
            clientes.Remove(cliente);
        }
        //void RelayMensaje(TcpClient cliente, byte[] buuffer)
        //{
        //    foreach (var item in clientes)
        //    {
        //        if (item != cliente)//Enviar a todos menos al origen
        //        {
        //            var ns = item.GetStream();
        //            ns.Write(buuffer, 0, buuffer.Length);
        //            ns.Flush();

        //        }
        //    }
        //}
        public void Detener()
        {
            try
            {
                if (server != null)
                {
                    server.Stop();
                    foreach (var item in clientes)
                    {
                        item.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }
    }
}
