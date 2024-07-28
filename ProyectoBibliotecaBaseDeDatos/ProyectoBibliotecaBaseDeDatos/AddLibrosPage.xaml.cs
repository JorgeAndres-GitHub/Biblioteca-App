using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
    /// Lógica de interacción para AddLibrosPage.xaml
    /// </summary>
    public partial class AddLibrosPage : Page
    {
        SqlConnection sqlConnection;
        public AddLibrosPage()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbNombre.Text) &&
                !string.IsNullOrWhiteSpace(tbAutor.Text) )
            {
                try
                {
                    sqlConnection.Open();

                    string consultaCountLibros = "SELECT COUNT(*) FROM Libros";
                    SqlCommand sqlCommandCountLibros = new SqlCommand(consultaCountLibros, sqlConnection);
                    int countLibrosEnEstanteria = (int)sqlCommandCountLibros.ExecuteScalar();

                    string consulta = "insert into Libros (NombreLibro, AutorLibro, EstadoLibro, EstanteriaId) values (@NombreLibro, @AutorLibro, @EstadoLibro, @EstanteriaId)";
                    SqlCommand sqlCommandInsertarLibro = new SqlCommand(consulta, sqlConnection);

                    sqlCommandInsertarLibro.Parameters.AddWithValue("@NombreLibro", tbNombre.Text);
                    sqlCommandInsertarLibro.Parameters.AddWithValue("@AutorLibro", tbAutor.Text);
                    sqlCommandInsertarLibro.Parameters.AddWithValue("@EstadoLibro", "Disponible");
                    sqlCommandInsertarLibro.Parameters.AddWithValue("@EstanteriaId", ++countLibrosEnEstanteria);
                    sqlCommandInsertarLibro.ExecuteScalar();
                    MessageBox.Show("Se ha agregado el libro");

                }
                catch (SqlException)
                {
                    MessageBox.Show("No hay espacio disponible en las estanterias");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("No puede dejar campos vacios");
            }
        }
    }
}
