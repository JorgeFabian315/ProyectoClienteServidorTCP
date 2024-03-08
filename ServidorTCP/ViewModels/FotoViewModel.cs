using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ServidorTCP.Models.Dtos;
using ServidorTCP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServidorTCP.ViewModels
{
    public partial class FotoViewModel:ObservableObject
    {
        public FotosService Server { get; set; } = new();

        public ObservableCollection<string> Usuarios { get; set; } = new();
        public ObservableCollection<FotoDto> Fotos { get; set; } = new();
        public string IP { get; set; } = "0.0.0.0";
        public int NumFoto { get; set; }

        public FotoViewModel()
        {
            GenerarIP();
            Server.RecibirFotoEvent += Server_RecibirFotoEvent;
        }

        private void Server_RecibirFotoEvent(object? sender, FotoDto e)
        {
            if (e.Foto == "**HELLO")
            {
                e.Foto = $"{e.Origen} se ha conectado";
                Usuarios.Add(e.Origen);
            }
            else if (e.Foto == "**BYE")
            {
                e.Foto = $"{e.Origen} se ha desconectado";
                Usuarios.Remove(e.Origen);
            }
            //Mensajes.Add(e);
            //NumMensaje = Mensajes.Count - 1;
            //PropertyChanged?.Invoke(this, new(nameof(NumMensaje)));
        }

        void GenerarIP()
        {
            var direcciones = Dns.GetHostAddresses(Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(x => x.ToString()));
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
