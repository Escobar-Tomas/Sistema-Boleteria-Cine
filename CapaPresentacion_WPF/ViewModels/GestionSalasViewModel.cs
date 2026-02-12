using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces; // Asegúrate de incluir la carpeta de interfaces
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionSalasViewModel : ObservableObject
    {
        // Usamos la interfaz en lugar de la clase concreta
        private readonly ICN_Sala _negocio;

        // Colección enlazada a la DataGrid
        public ObservableCollection<Sala> ListaSalas { get; set; } = new ObservableCollection<Sala>();

        // Propiedades del Formulario
        [ObservableProperty]
        private string nombreSala;

        [ObservableProperty]
        private int capacidadSala;

        [ObservableProperty]
        private Sala salaSeleccionada;

        // CAMBIO: Inyectamos la dependencia a través del constructor
        public GestionSalasViewModel(ICN_Sala negocio)
        {
            _negocio = negocio;
            CargarSalas();
        }

        private void CargarSalas()
        {
            ListaSalas.Clear();
            var lista = _negocio.Listar();
            foreach (var s in lista) ListaSalas.Add(s);
        }

        [RelayCommand]
        public void Guardar()
        {
            var sala = new Sala
            {
                Nombre = NombreSala,
                Capacidad = CapacidadSala,
                Estado = true
            };

            // Si hay una seleccionada, es edición (asumimos lógica simple por ahora)
            // Para simplificar, aquí siempre crea nueva. Luego podemos agregar edición.

            string resultado = _negocio.Guardar(sala);
            MessageBox.Show(resultado);

            if (resultado.Contains("correctamente"))
            {
                NombreSala = "";
                CapacidadSala = 0;
                CargarSalas();
            }
        }

        [RelayCommand]
        public void Eliminar(Sala sala)
        {
            if (sala == null) return;

            if (MessageBox.Show($"¿Eliminar {sala.Nombre}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _negocio.Eliminar(sala.Id);
                CargarSalas();
            }
        }
    }
}