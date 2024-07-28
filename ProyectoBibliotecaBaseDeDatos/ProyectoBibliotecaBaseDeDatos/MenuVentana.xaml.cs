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
using System.Windows.Shapes;

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para MenuVentana.xaml
    /// </summary>
    public partial class MenuVentana : Window
    {
        public int idUsuarioLogueado;
        public MenuVentana(int idUsuarioLogueado)
        {
            InitializeComponent();
            this.idUsuarioLogueado = idUsuarioLogueado;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            fMiMenu.Navigate(new Menu(idUsuarioLogueado));
        }
    }
}
