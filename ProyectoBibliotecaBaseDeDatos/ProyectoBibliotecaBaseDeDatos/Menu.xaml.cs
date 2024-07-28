using System;
using System.Collections.Generic;
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

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {        
        public int idUsuarioLogueado;
        public Menu(int idUsuarioLogueado)
        {
            this.idUsuarioLogueado = idUsuarioLogueado;
            InitializeComponent();
        }

        private void AgregarLibros_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddLibrosPage());
        }

        private void BuscarLibros_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BuscarLibroPage(idUsuarioLogueado));
        }

        private void MiPerfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PerfilPage(idUsuarioLogueado));
        }
    }
}
