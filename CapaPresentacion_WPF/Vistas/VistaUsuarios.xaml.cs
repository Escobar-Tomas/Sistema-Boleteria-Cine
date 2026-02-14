using CapaPresentacion_WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CapaPresentacion_WPF.Vistas
{
    public partial class VistaUsuarios : UserControl
    {
        public VistaUsuarios()
        {
            InitializeComponent();

            // Suscribirse al evento Loaded para reiniciar la contraseña visual si se limpia el formulario
            this.Loaded += VistaUsuarios_Loaded;
        }

        private void VistaUsuarios_Loaded(object sender, RoutedEventArgs e)
        {
            // Limpiar la caja de contraseña visual al cargar
            txtPassword.Password = string.Empty;
        }

        // Evento que actualiza el ViewModel cada vez que el usuario escribe en la caja de password
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is GestionUsuariosViewModel vm)
            {
                vm.ClaveForm = txtPassword.Password;
            }
        }

        // Limpiar visualmente el PasswordBox al hacer click en Limpiar
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = string.Empty;
        }
    }
}