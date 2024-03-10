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

        public List<string> FotosEliminadas { get; set; } = new();

        [ObservableProperty]
        public FotoDto foto = new();

        [ObservableProperty]
        public int imagenReciente;

        [ObservableProperty]
        public bool iniciarServer = false;
        public string IP { get; set; } = "0.0.0.0";

        private readonly string _carpeta = $"{AppDomain.CurrentDomain.BaseDirectory}Imagenes";
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
            else if (e.Estado == "**ELIMINAR")
            {
                var fotoEliminar = Fotos.Where(x => x.Id.ToLower() == e.Id.ToLower()).FirstOrDefault();
                if (fotoEliminar != null)
                {
                    FotosEliminadas.Add(fotoEliminar.Foto);
                    Fotos.Remove(fotoEliminar);
                }

                var fp = Fotos.FirstOrDefault();
                if (fp != null)
                    ImagenReciente = Fotos.IndexOf(fp);

                GuardarArchivo();
            }
            if (!string.IsNullOrWhiteSpace(e.Foto))
            {
               // e.Foto = DecodificarFoto(e);
                Fotos.Add(e);
                ImagenReciente = Fotos.IndexOf(e);
                GuardarArchivo();
            }

        }

        string DecodificarFoto(FotoDto foto)
        {
            byte[] imageBytes = Convert.FromBase64String(foto.Foto);

            if (!Directory.Exists(_carpeta))
                Directory.CreateDirectory(_carpeta);

            var totalFoto = Fotos
                .Where(x => x.Usuario.ToLower() == foto.Usuario.ToLower())
                .Count();

            string rutaImagen = $"{_carpeta}/{foto.Id}.jpg";

            //File.WriteAllBytes(rutaImagen, imageBytes);

            using (FileStream fs = new FileStream(rutaImagen, FileMode.Create))
            {
                fs.Write(imageBytes, 0, imageBytes.Length);
                fs.Close();
            }

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
            IniciarServer = true;
        }

        [RelayCommand]
        public void Detener()
        {
            //Fotos.Clear();
            Usuarios.Clear();
            Server.Detener();
            IniciarServer = false;

        }

    }
}
