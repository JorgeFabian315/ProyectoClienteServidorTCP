using ClienteTCP.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteTCP.Views
{
    /// <summary>
    /// Lógica de interacción para FotosView.xaml
    /// </summary>
    public partial class FotosView : UserControl
    {
        public FotosView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Todos los archivos de imagen | *.jpg;*.jpeg;*.png;";

            if (openFileDialog.ShowDialog() == true)
            {
                string rutaImagen = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(rutaImagen);
                long fileSizeInKB = fileInfo.Length / 1024; // Tamaño en KB
                if (fileSizeInKB > 3000)
                {
                    MessageBox.Show("La imagen debe ser menor o igual a 3 MB.", "Tamaño de imagen excedido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var vm = this.DataContext as FotoViewModel;
                if (vm != null)
                    vm.EnviarFotoCommand.Execute(rutaImagen);
            }
        }
    }
}

