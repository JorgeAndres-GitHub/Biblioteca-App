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
using System.Windows.Shapes;

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para EliminarCuentaWindow.xaml
    /// </summary>
    public partial class EliminarCuentaWindow : Window
    {
        SqlConnection sqlConnection;
        private int idUsuario;
        public EliminarCuentaWindow(int idUsuario)
        {
            InitializeComponent();
            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.idUsuario = idUsuario;

            string conexion = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(conexion);
        }

        private async void Si_Click(object sender, RoutedEventArgs e)
        {
            SqlTransaction sqlTransaction = null;
            try
            {
                sqlConnection.Open();

                sqlTransaction = sqlConnection.BeginTransaction();

                //actualizar estado del libro
                string consulta = "UPDATE Libros SET Libros.EstadoLibro = @estadoLibro " +
                    "FROM Libros INNER JOIN LibrosPrestados " +
                    "ON Libros.Id = LibrosPrestados.IdLibro " +
                    "WHERE LibrosPrestados.IdUsuario = @idUsuario ";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection, sqlTransaction);
                sqlCommand.Parameters.AddWithValue("@estadoLibro", "Disponible");
                sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                sqlCommand.ExecuteNonQuery();


                //Eliminar usuario de la tabla de usuarios
                string consulta2 = "DELETE FROM Usuarios WHERE Id=@id";
                SqlCommand sqlCommand2 = new SqlCommand(consulta2, sqlConnection ,sqlTransaction);
                
                sqlCommand2.Parameters.AddWithValue("@id", idUsuario);
                sqlCommand2.ExecuteNonQuery();

                sqlTransaction.Commit();    

                MessageBox.Show("Se ha eliminado la cuenta, todos los libros han sido devueltos automaticamente, cerraremos la sesión");

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        // Establecer la MainWindow como la ventana principal
                        Application.Current.MainWindow = mainWindow;
                    });
                });

                List<Window> windowsToClose = new List<Window>();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != Application.Current.MainWindow)
                    {
                        windowsToClose.Add(window);
                    }
                }

                foreach (Window window in windowsToClose)
                {
                    window.Close();
                }                             

            }
            catch (SqlException ex)
            {
                if (sqlTransaction != null && sqlTransaction.Connection != null)
                {
                    sqlTransaction.Rollback();
                }
                MessageBox.Show("Error en la base de datos: " + ex.Message);
            }
            catch (Exception ex)
            {
                if (sqlTransaction != null && sqlTransaction.Connection != null)
                {
                    sqlTransaction.Rollback();
                }
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
