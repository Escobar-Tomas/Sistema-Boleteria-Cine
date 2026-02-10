using System.Windows;
using CapaNegocio;
using CapaEntidad;

namespace CapaPresentacion_WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            CN_Usuario negocio = new CN_Usuario(App.CadenaConexion);

            // Validamos las credenciales
            Usuario usuarioEncontrado = negocio.Login(txtCorreo.Text, txtClave.Password);

            if (usuarioEncontrado != null)
            {
                // ¡ÉXITO! Abrimos el Dashboard y le pasamos el usuario
                MainWindow dashboard = new MainWindow(usuarioEncontrado);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Correo o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}