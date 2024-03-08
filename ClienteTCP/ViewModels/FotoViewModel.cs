using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClienteTCP.ViewModels
{
    public class FotoViewModel:INotifyPropertyChanged
    {
        public bool Conectado { get; set; } = true;
        public ICommand EnviarFotoCommang { get; set; }
        public FotoViewModel()
        {
            EnviarFotoCommang = new RelayCommand(Enviar);
        }

        private void Enviar()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
