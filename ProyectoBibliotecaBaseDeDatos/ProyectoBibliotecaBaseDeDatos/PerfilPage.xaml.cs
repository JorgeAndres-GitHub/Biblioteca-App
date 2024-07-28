using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Threading;

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para PerfilPage.xaml
    /// </summary>
    public partial class PerfilPage : Page
    {
        SqlConnection sqlConnection;
        public int idUsuario;
        private string nombreUsuario;
        private string correoUsuario;
        string conexion;

        public PerfilPage(int idUsuario)
        {
            InitializeComponent();
            this.idUsuario= idUsuario;
            conexion = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            MostrarDatosUsuario();
            MostrarDatosTablaLibrosPrestados();
            MostrarNotificacion();
        }

        /// <summary>
        /// Mostrar los datos basicos del usuario
        /// </summary>
        private void MostrarDatosUsuario()
        {
            using (SqlConnection sqlConnectionLocal = new SqlConnection(conexion))
            {
                try
                {
                    sqlConnectionLocal.Open();
                    string consulta = "SELECT Nombre, Correo FROM Usuarios WHERE Id=@id";
                    SqlCommand command = new SqlCommand(consulta, sqlConnectionLocal);
                    command.Parameters.AddWithValue("@id", idUsuario);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nombreUsuario = reader["Nombre"].ToString();
                            correoUsuario = reader["Correo"].ToString();
                        }
                    }
                    tbNombreUsuario.Text = nombreUsuario;
                    tbCorreoUsuario.Text = correoUsuario;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error en la base de datos:" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    sqlConnectionLocal.Close();
                }
            }
            
        }

        /// <summary>
        /// Mostrar los libros prestados por el usuario en el listview
        /// </summary>
        private void MostrarDatosTablaLibrosPrestados()
        {
            using (SqlConnection sqlConnectionLocal = new SqlConnection(conexion))
            {
                try
                {
                    string consulta = "SELECT Libros.Id, Libros.NombreLibro, Libros.AutorLibro, LibrosPrestados.FechaPrestamo, LibrosPrestados.FechaDevolucion " +
                        "FROM Libros INNER JOIN LibrosPrestados " +
                        "ON Libros.Id=LibrosPrestados.IdLibro " +
                        "WHERE LibrosPrestados.IdUsuario=@idUsuario";
                    sqlConnectionLocal.Open();
                    SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnectionLocal);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                    DataTable dataTable = new DataTable();
                    using (sqlDataAdapter)
                    {
                        sqlDataAdapter.Fill(dataTable);
                        lvLibrosPrestados.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error en la base de datos: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    sqlConnectionLocal.Close();
                }
            }
            
        }

        /// <summary>
        /// Evento para devolver el libro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Devolver_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el botón que se ha presionado
            Button button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Error: El control no es un botón");
                return;
            }

            // Obtener el DataContext del botón (la fila correspondiente)
            var libro = button.DataContext; // Asegúrate de que 'Libro' es el tipo correcto
            if (libro == null)
            {
                MessageBox.Show("Error: No se pudo obtener el libro de la fila");
                return;
            }

            DataRowView data = (DataRowView)libro;
            int id = (int)data["Id"];


            using (SqlConnection sqlConnectionLocal=new SqlConnection(conexion))
            {
                // Crear la conexión y la transacción
                SqlTransaction sqlTransaction = null;

                try
                {
                    // Abrir la conexión
                    sqlConnectionLocal.Open();

                    // Iniciar la transacción
                    sqlTransaction = sqlConnectionLocal.BeginTransaction();

                    // Actualizar el estado del libro a "Disponible"
                    string query1 = "UPDATE Libros SET EstadoLibro = @estadoLibro WHERE Id = @id";
                    using (SqlCommand sqlCommand1 = new SqlCommand(query1, sqlConnectionLocal, sqlTransaction))
                    {
                        sqlCommand1.Parameters.AddWithValue("@estadoLibro", "Disponible");
                        sqlCommand1.Parameters.AddWithValue("@id", id);
                        sqlCommand1.ExecuteNonQuery();
                    }

                    // Eliminar de la tabla de libros prestados del usuario
                    string query2 = "DELETE FROM LibrosPrestados WHERE IdLibro = @id";
                    using (SqlCommand sqlCommand2 = new SqlCommand(query2, sqlConnectionLocal, sqlTransaction))
                    {
                        sqlCommand2.Parameters.AddWithValue("@id", id);
                        sqlCommand2.ExecuteNonQuery();
                    }

                    // Confirmar la transacción
                    sqlTransaction.Commit();

                    // Notificar al usuario
                    MessageBox.Show("Se ha devuelto el libro");
                    
                }
                catch (SqlException ex)
                {
                    // Si ocurre un error, revertir la transacción
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                    }
                    MessageBox.Show("Error en la base de datos: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, revertir la transacción
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                    }
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    sqlConnectionLocal.Close();
                    MostrarDatosTablaLibrosPrestados();
                }
                
            }
        }

        /// <summary>
        /// Cerrar la ventana actual y abrir la ventana de inicio de sesión
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inicioSesion = new MainWindow();
            Window.GetWindow(this).Close();            
            inicioSesion.Show();
        }

        /// <summary>
        /// Abrir la ventana para confirmar la eliminacion de la cuenta del usuario logueado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarCuenta_Click(object sender, RoutedEventArgs e)
        {
            EliminarCuentaWindow ecw = new EliminarCuentaWindow(idUsuario);
            ecw.Show();
        }

        /// <summary>
        /// Mostrar una notificacion en caso de que se deba devolver algun libro en la fecha actual
        /// </summary>
        private void MostrarNotificacion()
        {
            new Thread(() =>
            {
                using (SqlConnection sqlConnectionLocal = new SqlConnection(conexion))
                {
                    try
                    {

                        sqlConnectionLocal.Open();
                        string query = "SELECT Libros.Id, Libros.NombreLibro, Libros.AutorLibro, LibrosPrestados.FechaDevolucion " +
                            "FROM Libros INNER JOIN LibrosPrestados " +
                            "ON Libros.Id=LibrosPrestados.IdLibro " +
                            "WHERE LibrosPrestados.IdUsuario=@idUsuario";

                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionLocal);
                        sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nombreLibro = reader["NombreLibro"].ToString();
                                string autorLibro = reader["AutorLibro"].ToString();
                                DateTime fechaDeDevolucion = (DateTime)reader["FechaDevolucion"];

                                if (fechaDeDevolucion == DateTime.Today)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        string fechaDeDevolucionFormateada = fechaDeDevolucion.ToString("dd/MM/yyyy");
                                        Thread.Sleep(1000);
                                        MessageBox.Show($"El libro {nombreLibro} de {autorLibro} tiene que devolverlo máximo hoy {fechaDeDevolucionFormateada}");
                                    });
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error en la base de datos: " + ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.ToString());
                    }
                    finally
                    {
                        sqlConnectionLocal.Close();
                    }
                }
                
            }).Start();
            
        }
    }
}
