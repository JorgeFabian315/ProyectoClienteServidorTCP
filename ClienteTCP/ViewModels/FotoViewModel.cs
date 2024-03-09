using ClienteTCP.Models.Dtos;
using ClienteTCP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClienteTCP.ViewModels
{
    public class FotoViewModel : INotifyPropertyChanged
    {
        public bool Conectado { get; set; }
        public ObservableCollection<FotoDto> Fotos { get; set; } = new ObservableCollection<FotoDto>();
        public List<string> Errores { get; set; } = new();
        public ICommand ConectarCommand { get; set; }
        public ICommand EnviarFotoCommand { get; set; }
        public string IP { get; set; } = "192.168.1.202";

        public ICommand EliminarFotoCommand => new RelayCommand(EliminarFoto);
        public ICommand VerEliminarFotoCommand => new RelayCommand<FotoDto>(VerEliminarFoto);
        public ICommand OcultarEliminarFotoCommand => new RelayCommand(OcultarEliminarFoto);

      

        public FotoDto Foto { get; set; } = new();
        public bool DeseaEliminarFoto { get; set; } = false;


        ComunicacionService cliente = new();

        public FotoViewModel()
        {
            ConectarCommand = new RelayCommand(Conectar);
            EnviarFotoCommand = new RelayCommand<string>(EnviarFoto);
            CargarArchivo();
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

        public void EnviarFoto(string? ruta)
        {
            try
            {
                if (ruta != null)
                {
                    var foto = new FotoDto()
                    {
                        Id = DateTime.Now.ToString("yy-MM-dd-HH:mm:ss").Replace(":", "-"),
                        Fecha = DateTime.Now,
                        Usuario = cliente.Equipo,
                        Foto = ruta,
                        Estado = "**Foto"
                    };

                    cliente.EnviarFoto(foto);

                    Fotos.Add(foto);
                    GuardarArchivo();
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }

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

        private void VerEliminarFoto(FotoDto? foto)
        {
            if (foto != null)
            {
                Foto = foto;
                DeseaEliminarFoto = true;

                OnPropertyChanged(nameof(Foto));    
                OnPropertyChanged(nameof(DeseaEliminarFoto));    
            }

        }
        private void EliminarFoto()
        {
            if (Foto != null)
            {
                Foto.Estado = "**ELIMINAR";
                Foto.Foto = "";
                cliente.EliminarFoto(Foto);
                Fotos.Remove(Foto);
                GuardarArchivo();
                OcultarEliminarFoto();

            }
        }

        private void OcultarEliminarFoto()
        {
            DeseaEliminarFoto = false;
            OnPropertyChanged(nameof(DeseaEliminarFoto));
        }

        public void OnPropertyChanged(string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
