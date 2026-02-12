using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaEntidad;
using Microsoft.Extensions.DependencyInjection;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _vistaActual;

        private Usuario _usuario;

        // Propiedades para mostrar en la UI
        public string NombreUsuario => _usuario?.NombreCompleto ?? "Usuario Desconocido";
        public string RolUsuario => _usuario?.Rol ?? "Sin Rol";

        // Propiedad que controla si se ve el botón de Usuarios
        public bool EsAdministrador => _usuario?.Rol == "Administrador";

        public MainViewModel(Usuario usuario)
        {
            _usuario = usuario;

            MostrarVentas(); // Por defecto, es mejor mostrar Ventas al iniciar
        }

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
            VistaActual = App.ServiceProvider.GetRequiredService<VentasViewModel>();
        }

        // NUEVO COMANDO: Navegar a Usuarios
        [RelayCommand]
        public void MostrarUsuarios()
        {
            VistaActual = App.ServiceProvider.GetRequiredService<GestionUsuariosViewModel>();
        }
    }
}