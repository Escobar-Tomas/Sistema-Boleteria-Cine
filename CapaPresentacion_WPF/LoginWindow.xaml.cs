using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Necesario para mover la ventana
using CapaNegocio.Interfaces;
using CapaEntidad;
using Microsoft.Extensions.DependencyInjection;

namespace CapaPresentacion_WPF
{
    public partial class LoginWindow : Window
    {
        private readonly ICN_Usuario _negocioUsuario;
        private bool _isPasswordChanging; // Bandera para evitar bucles infinitos

        public LoginWindow()
        {
            InitializeComponent();
            _negocioUsuario = App.ServiceProvider.GetRequiredService<ICN_Usuario>();
        }

        // Evento para mover la ventana al hacer click izquierdo (necesario por WindowStyle="None")
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos la contraseña real dependiendo de cuál caja esté visible
            string passwordFinal = txtClave.Visibility == Visibility.Visible ? txtClave.Password : txtClaveVisible.Text;

            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || string.IsNullOrWhiteSpace(passwordFinal))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Usuario usuarioEncontrado = _negocioUsuario.Login(txtCorreo.Text, passwordFinal);

            if (usuarioEncontrado != null)
            {
                MainWindow dashboard = new MainWindow(usuarioEncontrado);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Correo o contraseña incorrectos", "Error de Acceso", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Lógica para mostrar/ocultar contraseña
        private void btnTogglePass_Click(object sender, RoutedEventArgs e)
        {
            if (btnTogglePass.IsChecked == true)
            {
                // Mostrar contraseña
                txtClaveVisible.Text = txtClave.Password;
                txtClaveVisible.Visibility = Visibility.Visible;
                txtClave.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Ocultar contraseña
                txtClave.Password = txtClaveVisible.Text;
                txtClaveVisible.Visibility = Visibility.Collapsed;
                txtClave.Visibility = Visibility.Visible;
            }
        }

        // Sincronizar PasswordBox -> TextBox
        private void txtClave_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordChanging && txtClave.Visibility == Visibility.Visible)
            {
                _isPasswordChanging = true;
                txtClaveVisible.Text = txtClave.Password;
                _isPasswordChanging = false;
            }
        }

        // Sincronizar TextBox -> PasswordBox
        private void txtClaveVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isPasswordChanging && txtClaveVisible.Visibility == Visibility.Visible)
            {
                _isPasswordChanging = true;
                txtClave.Password = txtClaveVisible.Text;
                _isPasswordChanging = false;
            }
        }
    }
}