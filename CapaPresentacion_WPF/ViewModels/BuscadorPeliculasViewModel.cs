using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio;
using CapaNegocio.ModelosAPI;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class BuscadorPeliculasViewModel : ObservableObject
    {
        // Referencia a nuestra lógica de negocio
        private readonly CN_Pelicula _cnPelicula;

        // Propiedades enlazadas a la Vista (Binding)
        [ObservableProperty]
        private string textoBusqueda;

        [ObservableProperty]
        private bool estaBuscando; // Para mostrar una barrita de carga

        // Usamos ObservableCollection para que la lista en pantalla se actualice sola
        public ObservableCollection<PeliculaBusqueda> Resultados { get; set; } = new ObservableCollection<PeliculaBusqueda>();

        [ObservableProperty]
        private PeliculaBusqueda peliculaSeleccionada;

        // Constructor: Recibimos la lógica de negocio ya configurada
        public BuscadorPeliculasViewModel(CN_Pelicula cnPelicula)
        {
            _cnPelicula = cnPelicula;
        }

        // COMANDO: Buscar Película
        [RelayCommand]
        public async Task Buscar()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda)) return;

            EstaBuscando = true;
            Resultados.Clear();

            var lista = await _cnPelicula.BuscarPeliculasEnTMDB(TextoBusqueda);

            foreach (var item in lista)
            {
                Resultados.Add(item);
            }

            EstaBuscando = false;
        }

        // COMANDO: Guardar Película Seleccionada en BD
        [RelayCommand]
        public async Task Guardar()
        {
            if (PeliculaSeleccionada == null) return;

            EstaBuscando = true; // Reusamos el indicador de carga

            // Llamamos al método que creamos en la CapaNegocio que descarga detalles y guarda
            string mensaje = await _cnPelicula.GuardarPeliculaDesdeApi(PeliculaSeleccionada.Id);

            MessageBox.Show(mensaje);

            EstaBuscando = false;
        }
    }
}