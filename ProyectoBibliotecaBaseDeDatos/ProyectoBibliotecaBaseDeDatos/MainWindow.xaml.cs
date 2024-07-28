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
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Runtime.Remoting.Messaging;

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string connectionString = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);            
        }

        private void NoTengoCuenta_Checked(object sender, RoutedEventArgs e)
        {
            RegistroUsuarios registroUsuarios = new RegistroUsuarios();
            this.Close();
            registroUsuarios.Show();            
        }

        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarInicioSesion(tbMiCorreo.Text, pbMiContrasenia.Password))
            {
                MessageBox.Show("Ha iniciado sesion correctamente");
                int idUsuarioLogueado = -1;
                try
                {
                    string query = "Select Id from Usuarios where Correo=@correo";
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@correo", tbMiCorreo.Text);
                    using (SqlDataReader reader2 = command.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            idUsuarioLogueado = (int)reader2["Id"];
                        }
                    }
                } catch (SqlException ex)
                {
                    MessageBox.Show("Error en la base de datos:" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
                if (idUsuarioLogueado != -1)
                {
                    MenuVentana menuVentana = new MenuVentana(idUsuarioLogueado);
                    this.Close();
                    menuVentana.Show();
                }
                else
                {
                    MessageBox.Show("No se encontró el usuario.");
                }
                
                
            }
            else
            {
                MessageBox.Show("El correo y/o la contraseña son incorrectos");
            }
        }

        private bool ValidarInicioSesion(string correo, string contraseniaIngresada)
        {
            bool validado = false;
            try
            {                
                string consultaContrasenia = "select Correo, Contrasenia from Usuarios where Correo=@correo";

                using (SqlCommand sqlCommand = new SqlCommand(consultaContrasenia, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@correo", correo);

                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string contraseniaDesdeBD = reader["Contrasenia"].ToString();
                            string contraseniaHasheada = HashPassword(contraseniaIngresada);
                            if (contraseniaDesdeBD == contraseniaHasheada)
                            {
                                validado = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return validado;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void ContraseniaOlvidada_Checked(object sender, RoutedEventArgs e)
        {
            CambioContraseña cambioContraseña = new CambioContraseña(tbMiCorreo.Text);
            cambioContraseña.Show();
        }
    }
}
