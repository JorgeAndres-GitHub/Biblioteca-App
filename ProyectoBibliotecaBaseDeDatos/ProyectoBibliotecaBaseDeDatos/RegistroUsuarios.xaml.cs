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
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ProyectoBibliotecaBaseDeDatos
{
    /// <summary>
    /// Lógica de interacción para RegistroUsuarios.xaml
    /// </summary>
    public partial class RegistroUsuarios : Window
    {
        SqlConnection sqlConnection;
        
        public RegistroUsuarios()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string connectionString = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
        }

        private void Registrarse_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbNombreRegistrar.Text) &&
                !string.IsNullOrWhiteSpace(tbCorreoRegistrar.Text) &&
                !string.IsNullOrWhiteSpace(pbContraseña1.Password) &&
                !string.IsNullOrWhiteSpace(pbContraseña2.Password))
            {
                string patronCorreo = @"^[a-zA-Z0-9]+@gmail\.com$";
                if (Regex.IsMatch(tbCorreoRegistrar.Text, patronCorreo))
                {
                    if (pbContraseña1.Password == pbContraseña2.Password)
                    {
                        string hashedPassword = HashPassword(pbContraseña1.Password);
                        try
                        {
                            string consulta = "insert into Usuarios values (@Nombre, @Correo, @Contrasenia)";
                            SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@Nombre", tbNombreRegistrar.Text);
                            sqlCommand.Parameters.AddWithValue("@Correo", tbCorreoRegistrar.Text);
                            sqlCommand.Parameters.AddWithValue("@Contrasenia", hashedPassword);                            
                            sqlCommand.ExecuteScalar();
                            MessageBox.Show("Se inserto correctamente");
                        }
                        catch(SqlException ex)
                        {
                            MessageBox.Show($"Error en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error general {ex.Message}");
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Las contraseñas no coinciden");
                    }
                }
                else
                {
                    MessageBox.Show("El correo ingresado no es valido");
                }
            }
            else
            {
                MessageBox.Show("No puede dejar ningun dato personal vacio");
            }
        }

        private void Atras_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inicioSesionVentana = new MainWindow();
            this.Close();
            inicioSesionVentana.Show();        
        }

        private static string HashPassword(string password)
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

    }
}
