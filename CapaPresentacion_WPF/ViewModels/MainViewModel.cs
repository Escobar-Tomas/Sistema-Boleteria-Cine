using CapaEntidad;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using CapaPresentacion_WPF.ViewModels;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _vistaActual;

        // MANTENEMOS TU LÓGICA DE USUARIO INTACTA
        private Usuario _usuario;

        public string NombreUsuario => _usuario?.NombreCompleto ?? "Usuario Desconocido";
        public string RolUsuario => _usuario?.Rol ?? "Sin Rol";

        // Propiedad que controla si se ve el botón de Usuarios
        public bool EsAdministrador => _usuario?.Rol == "Administrador";

        // Constructor que recibe el usuario del Login
        public MainViewModel(Usuario usuario)
        {
            _usuario = usuario;

            // Iniciar en la pantalla de Ventas
            MostrarVentas();
        }

        // --- MÉTODOS DE NAVEGACIÓN CORREGIDOS (Factory Pattern) ---
        // Usamos GetRequiredService para generar una instancia NUEVA y LIMPIA cada vez.

        [RelayCommand]
        public void MostrarPeliculas()
        {
            VistaActual = App.ServiceProvider.GetRequiredService<BuscadorPeliculasViewModel>();
        }

        [RelayCommand]
        public void MostrarSalas()
        {
            VistaActual = App.ServiceProvider.GetRequiredService<GestionSalasViewModel>();
        }

        [RelayCommand]
        public void MostrarFunciones()
        {
            VistaActual = App.ServiceProvider.GetRequiredService<GestionFuncionesViewModel>();
        }

        [RelayCommand]
        public void MostrarVentas()
        {
            // ALERTA: Esta es la línea clave que arregla el bug de "funciones desaparecidas".
            // Crea un VentasViewModel nuevo con un DbContext nuevo.
            VistaActual = App.ServiceProvider.GetRequiredService<VentasViewModel>();
        }

        [RelayCommand]
        public void MostrarUsuarios()
        {
            VistaActual = App.ServiceProvider.GetRequiredService<GestionUsuariosViewModel>();
        }

        [RelayCommand]
        public void CerrarSesion()
        {
            // Lógica opcional para cerrar la ventana actual y abrir el Login
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Cerrar la ventana principal actual
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}