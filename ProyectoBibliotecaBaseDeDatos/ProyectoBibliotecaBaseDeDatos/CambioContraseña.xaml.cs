using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
    /// Lógica de interacción para CamioContraseña.xaml
    /// </summary>
    public partial class CambioContraseña : Window
    {
        SqlConnection sqlConnection;
        private string correo;
        public CambioContraseña(string correo)
        {
            InitializeComponent();
            this.correo = correo;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string conectionString = ConfigurationManager.ConnectionStrings["ProyectoBibliotecaBaseDeDatos.Properties.Settings.EsDirectaDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(conectionString);
        }

        private void ActualizarContrasenia_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(pbMiContraseñaActualizada1.Password) && !string.IsNullOrWhiteSpace(pbMiContraseñaActualizada2.Password))
            {
                if (pbMiContraseñaActualizada1.Password == pbMiContraseñaActualizada2.Password)
                {
                    string hashedPassword = HashPassword(pbMiContraseñaActualizada1.Password);
                    try
                    {
                        string consultaActualizacion = "Update Usuarios Set Contrasenia=@contrasenia where Correo=@correo";
                        SqlCommand sqlCommand = new SqlCommand(consultaActualizacion, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@correo", correo);
                        sqlCommand.Parameters.AddWithValue("@contrasenia", hashedPassword);
                        sqlCommand.ExecuteScalar();
                        MessageBox.Show("Contraseña actualizada");
                        this.Close();
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
                    MessageBox.Show("Las contraseñas no son iguales");
                }
            }
            else
            {
                MessageBox.Show("No puede dejar los campos vacios");
            }
            
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
    }
}
