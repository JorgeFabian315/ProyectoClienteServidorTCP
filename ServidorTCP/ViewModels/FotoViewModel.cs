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
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ServidorTCP.ViewModels
{
    public partial class FotoViewModel : ObservableObject
    {
        public FotosService Server { get; set; } = new();

        public ObservableCollection<string> Usuarios { get; set; } = new();
        public ObservableCollection<FotoDto> Fotos { get; set; } = new();

        public FotoDto Foto { get; set; } = new();
        public string IP { get; set; } = "0.0.0.0";
        public FotoViewModel()
        {
            GenerarIP();
            Server.RecibirFotoEvent += Server_RecibirFotoEvent;
        }

        private void Server_RecibirFotoEvent(object? sender, FotoDto e)
        {
            if (e.Estado == "**HELLO")
            {
                Usuarios.Add(e.Origen);
            }
            else if (e.Estado == "**BYE")
            {
                Usuarios.Remove(e.Origen);
            }

            //if (vm.Archivo == null) // No elijio archivo
            //{
            //    //obtener el id del producto
            //    //copiar el archivo nodisponible jpg y cambiar el nombre por el id
            //    System.IO.File.Copy("wwwroot/img_frutas/0.jpg", $"wwwroot/img_frutas/{vm.Producto.Id}.jpg");
            //}
            //else
            //{
            //    System.IO.FileStream fs = System.IO.File.Create($"wwwroot/img_frutas/{vm.Producto.Id}.jpg");
            //    vm.Archivo.CopyTo(fs);
            //    fs.Close();
            //}

        }

        string DecodificarImagen(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(e.Foto);

            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return "";
        }


        void GenerarIP()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(x => x.ToString())
                    .FirstOrDefault());
            }
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
