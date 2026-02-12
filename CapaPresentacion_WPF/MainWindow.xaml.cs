using System.Windows;
using System.Windows.Input;
using CapaEntidad;
using CapaPresentacion_WPF.ViewModels;

namespace CapaPresentacion_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            DataContext = new MainViewModel(usuario);
        }

        // Permitir arrastrar la ventana desde la barra superior
        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Minimizar
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Cerrar Aplicación (o volver al Login, según prefieras)
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}