﻿using ServidorTCP.Models.Dtos;
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
                    tcpClient.ReceiveBufferSize = 500000;
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
                var ns = cliente.GetStream();

                // Usamos una lista para almacenar los fragmentos del JSON
                var jsonFragments = new List<byte>();

                while (true)
                {
                    try { 
                        byte[] buffer = new byte[4096]; // Buffer de lectura
                        int bytesRead = ns.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            // No hay más datos disponibles, salimos del bucle
                            break;
                        }

                        // Almacenamos los datos leídos en la lista de fragmentos
                        jsonFragments.AddRange(buffer.Take(bytesRead));

                        // Si no hay más datos disponibles, terminamos la lectura
                        if (ns.DataAvailable == false)
                        {
                            break;
                        }
                    
                    }
                    catch (Exception ex)
                    {
                        Errores.Add(ex.Message);
                    }
                    // Intentamos leer datos del flujo de red
                }

                // Convertimos los fragmentos en una cadena JSON completa

                string json = Encoding.UTF8.GetString(jsonFragments.ToArray());

                // Deserializamos el JSON
                FotoDto foto=null;
                try
                {

                 foto = JsonSerializer.Deserialize<FotoDto>(json);
                }
                catch (Exception ex)
                {
                    Errores.Add(ex.Message);
                }

                if (foto != null)
                {
                    // Invocamos el evento en el hilo de la aplicación
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RecibirFotoEvent?.Invoke(this, foto);
                    });
                }
            }
            // Removemos el cliente de la lista de clientes después de que se desconecta
            clientes.Remove(cliente);
        }

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
