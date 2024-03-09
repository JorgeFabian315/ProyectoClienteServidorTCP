using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ServidorTCP.Models.Dtos;
using ServidorTCP.Services;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ServidorTCP.ViewModels
{
    public partial class FotoViewModel : ObservableObject
    {
        public FotosService Server { get; set; } = new();
        public ObservableCollection<string> Usuarios { get; set; } = new();
        public ObservableCollection<FotoDto> Fotos { get; set; } = new();

        [ObservableProperty]
        public FotoDto foto = new();

        [ObservableProperty]
        public int imagenReciente;
        public string IP { get; set; } = "0.0.0.0";
        public FotoViewModel()
        {
            GenerarIP();
            Server.RecibirFotoEvent += Server_RecibirFotoEvent;
            CargarArchivo();
        }

        private void Server_RecibirFotoEvent(object? sender, FotoDto e)
        {
            if (e.Estado == "**HELLO")
            {
                Usuarios.Add(e.Usuario);
            }
            else if (e.Estado == "**BYE")
            {
                Usuarios.Remove(e.Usuario);
            }

            if (!string.IsNullOrWhiteSpace(e.Foto))
            {
                e.Foto = DecodificarFoto(e);
                Fotos.Add(e);
                ImagenReciente = Fotos.IndexOf(e);
                GuardarArchivo();
            }

        }

        string DecodificarFoto(FotoDto foto)
        {
            byte[] imageBytes = Convert.FromBase64String(foto.Foto);

            // Guarda los bytes de la imagen en un archivo en el disco duro
            var carpeta = $"{AppDomain.CurrentDomain.BaseDirectory}ImagenesDecodificadas";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var totalFoto = Fotos
                .Where(x => x.Usuario.ToLower() == foto.Usuario.ToLower())
                .Count();

            string rutaImagen = $"{carpeta}//{foto.Usuario}_{totalFoto + 1}.jpg";

            File.WriteAllBytes(rutaImagen, imageBytes);

            return rutaImagen;
        }

        string nombreJson = "Fotos.json";
        public void GuardarArchivo()
        {
            var json = JsonSerializer.Serialize(Fotos);
            File.WriteAllText(nombreJson, json);
        }

        // Cargar el archivo json
        public void CargarArchivo()
        {
            if (File.Exists(nombreJson))
            {
                var json = File.ReadAllText(nombreJson);
                var datos = JsonSerializer.Deserialize<ObservableCollection<FotoDto>?>(json);
                if (datos != null)
                    Fotos = datos;
                else
                    Fotos = new ObservableCollection<FotoDto>();
            }
        }


        void GenerarIP()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(x => x.ToString()).FirstOrDefault());
            }
        }

        [RelayCommand]

        private void Iniciar()
        {
            Server.Iniciar();
        }

        [RelayCommand]
        public void Detener()
        {
            Fotos.Clear();
            Usuarios.Clear();
            Server.Detener();
        }


    }
}
