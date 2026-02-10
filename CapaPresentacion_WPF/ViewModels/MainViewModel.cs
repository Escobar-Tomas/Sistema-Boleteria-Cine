using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio;
using CapaEntidad;
using Microsoft.Extensions.Configuration;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Esta propiedad es la que el MainWindow "mira" para saber qué dibujar
        [ObservableProperty]
        private object _vistaActual;

        private Usuario _usuario;
        public string NombreUsuario => _usuario?.NombreCompleto;
        public string RolUsuario => _usuario?.Rol;

        // API Key (La leemos una vez aquí)
        private string _apiKey;

        public MainViewModel(Usuario usuario)
        {
            _usuario = usuario;
            CargarConfiguracion();

            // Por defecto, mostramos el módulo de películas al iniciar (para probar)
            MostrarPeliculas();
        }

        private void CargarConfiguracion()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<App>();
            var config = builder.Build();
            _apiKey = config["TmdbApiKey"] ?? "";
        }

        // COMANDO: Navegar a Películas
        [RelayCommand]
        public void MostrarPeliculas()
        {
            CN_Pelicula negocio = new CN_Pelicula(_apiKey, App.CadenaConexion);
            // "Inyectamos" el ViewModel de Películas en la vista actual
            VistaActual = new BuscadorPeliculasViewModel(negocio);
        }

        // COMANDO: Navegar a Ventas (Placeholder por ahora)
        [RelayCommand]
        public void MostrarVentas()
        {
            //VistaActual = new VentasViewModel(); // Descomentar cuando crees ese ViewModel
        }

        [RelayCommand]
        public void MostrarSalas()
        {
            // Cambiamos la vista actual al ViewModel de Salas
            VistaActual = new GestionSalasViewModel();
        }

        [RelayCommand]
        public void MostrarFunciones()
        {
            // Esta línea es la que permite que el ContentControl de MainWindow cambie la vista
            VistaActual = new GestionFuncionesViewModel();
        }
    }
}