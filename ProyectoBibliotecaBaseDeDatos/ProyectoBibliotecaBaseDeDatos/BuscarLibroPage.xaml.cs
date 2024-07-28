using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para BuscarLibroPage.xaml
    /// </summary>
    public partial class BuscarLibroPage : Page, INotifyPropertyChanged
    {
        SqlConnection conn;
        public int idUsuarioLogueado;

        //Actualizar texto inicial del textbox de busqueda
        private string texto;
        public string Texto {
            get => texto;
            set 
            {
                if (texto != value)
                {
                    texto = value;
                    OnPropertyChanged(nameof(Texto));
                }     
            } 
        }

        //Actualizar el texto del texbox de nombre del libro
        private string texto2="Seleccione un libro";
        public string Texto2
        {
            get => texto2;
            set 
            {
                if (texto2 != value)
                {
                    texto2 = value;
                    OnPropertyChanged(nameof(Texto2));
                }
            } 
        }

        //Actualizar el texto del texbox de nombre del autor
        private string texto3 = "Seleccione un libro";
        public string Texto3
        {
            get => texto3;
            set
            {
                if (texto3 != value)
                {
                    texto3 = value;
                    OnPropertyChanged(nameof(Texto3));
                }
            }
        }

        public BuscarLibroPage(int idUsuarioLogueado)
        {
            InitializeComponent();
            DataContext = this;
            this.idUsuarioLogueado = idUsuarioLogueado;

            string connectionString = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            conn = new SqlConnection(connectionString);            
            MostrarDatosTablaLibros();
            
        }

        /// <summary>
        /// Mostrar los datos de todos los libros en la base de datos
        /// </summary>
        private void MostrarDatosTablaLibros()
        {
            try
            {                
                string consulta = "SELECT Libros.Id, Libros.NombreLibro, Libros.AutorLibro, Libros.EstadoLibro, Estanterias.NumeroEstanteria, Estanterias.PisoEstanteria " +
                    "FROM Libros " +
                    "INNER JOIN Estanterias ON Libros.EstanteriaId=Estanterias.Id;";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(consulta, conn);
                DataTable dataTable=new DataTable();
                using (dataAdapter)
                {
                    dataAdapter.Fill(dataTable);                    
                    lvLibros.ItemsSource = dataTable.DefaultView;
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
        }

        /// <summary>
        /// Realizar busqueda dependiendo de lo ingresado por el usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            string terminoBuscado = "%" + tbLibroBuscar.Text + "%";

            //primer radiobutton
            if (rbNombrelibro.IsChecked == true)
            {

                try
                {
                    string consulta1 = "SELECT Libros.Id, Libros.NombreLibro, Libros.AutorLibro, Libros.EstadoLibro, Estanterias.NumeroEstanteria, Estanterias.PisoEstanteria " +
                                "FROM Libros " +
                                "INNER JOIN Estanterias ON Libros.EstanteriaId=Estanterias.Id " +
                                "WHERE Libros.NombreLibro Like @nombreLibro";
                    SqlCommand sqlCommand=new SqlCommand(consulta1, conn);
                    SqlDataAdapter sqlDataAdapter=new SqlDataAdapter(sqlCommand);
                    using (sqlDataAdapter)
                    {
                        sqlCommand.Parameters.AddWithValue("@nombreLibro", terminoBuscado);
                        DataTable dataTable=new DataTable();
                        sqlDataAdapter.Fill(dataTable);
                        lvLibros.ItemsSource=dataTable.DefaultView;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //segundo radiobutton
            else 
            {
                try
                {
                    string consulta2 = "SELECT Libros.Id, Libros.NombreLibro, Libros.AutorLibro, Libros.EstadoLibro, Estanterias.NumeroEstanteria, Estanterias.PisoEstanteria " +
                                            "FROM Libros " +
                                            "INNER JOIN Estanterias ON Libros.EstanteriaId=Estanterias.Id " +
                                            "WHERE Libros.AutorLibro Like @autorLibro";
                    SqlCommand sqlCommand = new SqlCommand(consulta2, conn);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    using (sqlDataAdapter)
                    {
                        sqlCommand.Parameters.AddWithValue("@autorLibro", terminoBuscado);
                        DataTable dataTable2 = new DataTable();
                        sqlDataAdapter.Fill(dataTable2);
                        lvLibros.ItemsSource = dataTable2.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// checkeo de los radiobutton para cambiar el texto del textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbNombrelibro.IsChecked == true)
            {
                Texto = "Escribe el nombre del libro que desea buscar";                              
            }
            else if (rbAutorLibro.IsChecked == true)
            {
                Texto = "Escribe el nombre del autor que desea buscar";               
            }
        }
        /// <summary>
        /// Verificar que fila esta seleccionada para cambiar lo mostrado en los textbox inferiores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvLibros.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)lvLibros.SelectedItem;
                Texto2 = selectedRow["NombreLibro"].ToString();
                Texto3 = selectedRow["AutorLibro"].ToString();
            }
        }

        /// <summary>
        /// reiniciar la pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recargar_Click(object sender, RoutedEventArgs e)
        {
            MostrarDatosTablaLibros();
        }

        /// <summary>
        /// elimina el contenido del textbox cuando el usuario va a empezar a escribir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbLibroBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            tbLibroBuscar.Clear();
        }

        /// <summary>
        /// Evento para prestar libros de la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prestar_Click(object sender, RoutedEventArgs e)
        {
            if (lvLibros.SelectedItem != null)
            {
                DataRowView data = (DataRowView)lvLibros.SelectedItem;
                int id = (int)data["Id"];

                if (data["EstadoLibro"].ToString()!="Prestado")
                {
                    //agregar a la tabla libros prestados
                    try
                    {
                        string query = "Insert into LibrosPrestados (IdLibro, IdUsuario, FechaPrestamo, FechaDevolucion) Values (@idLibro, @idUsuario, @fechaPrestamo, @fechaDevolucion)";
                        SqlCommand sqlCommand = new SqlCommand(query, conn);
                        conn.Open();
                        sqlCommand.Parameters.AddWithValue("@idLibro", id);
                        sqlCommand.Parameters.AddWithValue("@idUsuario", idUsuarioLogueado);
                        sqlCommand.Parameters.AddWithValue("@fechaPrestamo", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@fechaDevolucion", DateTime.Now.AddDays(3));
                        sqlCommand.ExecuteScalar();
                    }
                    catch (SqlException ex1)
                    {
                        MessageBox.Show("Error en la base de datos: " + ex1.Message);
                    }
                    catch (Exception ex2) 
                    {
                        MessageBox.Show("Error: " + ex2.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                    
                    
                    //actualizar estado del libro
                    try
                    {
                        string consulta = "Update Libros Set EstadoLibro=@estadoLibro where Id=@id";
                        SqlCommand sql = new SqlCommand(consulta, conn);
                        conn.Open();
                        sql.Parameters.AddWithValue("@estadoLibro", "Prestado");
                        sql.Parameters.AddWithValue("@id", id);
                        sql.ExecuteScalar();
                        MessageBox.Show("Se ha prestado el libro");
                        MostrarDatosTablaLibros();
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
                        conn.Close();
                    }
                }
                else
                {
                    MessageBox.Show("El libro se encuentra prestado");
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un libro primero");
            }
        }

        /// <summary>
        /// Evento para eliminar un libro de la biblioteca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lvLibros.SelectedItem != null)
            {
                DataRowView data = (DataRowView)lvLibros.SelectedItem;
                int id = (int)data["Id"];

                if (data["EstadoLibro"].ToString() != "Prestado")
                {                    
                    try
                    {
                        conn.Open();
                        string query1 = "DELETE FROM LIBROS WHERE Id=@id";
                        SqlCommand sqlCommand = new SqlCommand(query1, conn);
                        sqlCommand.Parameters.AddWithValue("@id", id);
                        sqlCommand.ExecuteScalar();
                        MessageBox.Show("El libro ha sido eliminado");
                        MostrarDatosTablaLibros();
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
                        conn.Close();
                    } 
                }
                else
                {
                    MessageBox.Show("El libro se encuentra prestado, no puede eliminarlo");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un libro primero");
            }
        }

        //Evento para verificar el cambio de propiedades y modificar el textbox
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }        
    }
}
