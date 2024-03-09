using ClienteTCP.Models.Dtos;
using ClienteTCP.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClienteTCP.ViewModels
{
    public class FotoViewModel:INotifyPropertyChanged
    {
        public bool Conectado { get; set; }
        public ObservableCollection<FotoDto> Fotos { get; } = new ObservableCollection<FotoDto>();
        public ICommand ConectarCommand {get; set; }
        public ICommand EliminarFotoCommand { get; set; }
        public string IP { get; set; } = "";
        ComunicacionService cliente = new();
        public FotoViewModel()
        {
            EliminarFotoCommand=new RelayCommand<FotoDto>(EliminarFoto);   
            ConectarCommand = new RelayCommand(Conectar);
        }

        private void EliminarFoto(FotoDto? dto)
        {
            if(dto != null)
            {
                Fotos.Remove(dto);
                cliente.Eliminar(dto);
            }
        }

        private void Conectar()
        {
            IPAddress.TryParse(IP, out IPAddress? ipAddress);
            if (ipAddress != null)
            {
                cliente.Conectar(ipAddress);
                Conectado = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conectado)));
            }
        }

        public void Enviar(string ruta)
        {
            try
            {
                // Cargar la imagen desde el archivo

                byte[] bytesImagen = File.ReadAllBytes(ruta);

                // Convertir a Base64
                string imagenBase64 = Convert.ToBase64String(bytesImagen);
                cliente.EnviarFoto(new FotoDto
                {
                    Base64EncodedImage = imagenBase64,
                    UserName = cliente.equipo,

                });

                Fotos.Add(new FotoDto
                {
                    Base64EncodedImage = imagenBase64,
                    UserName = cliente.equipo,
                });

            }
            catch (Exception ex)
            {
               
            }
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
