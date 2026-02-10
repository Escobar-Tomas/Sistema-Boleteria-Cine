using System.Windows;
using CapaEntidad;
using CapaPresentacion_WPF.ViewModels;

namespace CapaPresentacion_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            // Vinculamos la vista con el ViewModel Principal
            DataContext = new MainViewModel(usuario);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}