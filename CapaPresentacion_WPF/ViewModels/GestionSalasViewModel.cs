using CapaEntidad;
using CapaNegocio.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionSalasViewModel : ObservableObject
    {
        private readonly ICN_Sala _servicioSala;

        // Propiedades de la Lista
        [ObservableProperty]
        private ObservableCollection<Sala> _listaSalas = new ObservableCollection<Sala>();

        // Propiedades del Formulario
        [ObservableProperty]
        private Sala _salaSeleccionada;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CapacidadCalculada))]
        private int _filas;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CapacidadCalculada))]
        private int _columnas;

        public int CapacidadCalculada => Filas * Columnas;

        public GestionSalasViewModel(ICN_Sala servicioSala)
        {
            _servicioSala = servicioSala;
            // Carga inicial asíncrona (fire and forget seguro en constructor)
            _ = CargarSalas();
            Limpiar();
        }

        // Método de Carga Asíncrono
        public async Task CargarSalas()
        {
            try
            {
                var lista = await _servicioSala.ListarAsync();
                ListaSalas.Clear();
                foreach (var s in lista)
                {
                    ListaSalas.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar salas: {ex.Message}");
            }
        }

        // Al seleccionar una sala de la lista, llenamos el formulario
        partial void OnSalaSeleccionadaChanged(Sala value)
        {
            if (value != null)
            {
                Filas = value.Filas;
                Columnas = value.Columnas;
            }
            else
            {
                // Si se deselecciona, limpiamos (opcional, o mantener último valor)
            }
        }

        [RelayCommand]
        private async Task Guardar()
        {
            // Si es nueva sala (no seleccionada de la lista), creamos una instancia
            if (SalaSeleccionada == null)
            {
                SalaSeleccionada = new Sala();
            }

            // Mapeamos datos del formulario al objeto
            SalaSeleccionada.Filas = Filas;
            SalaSeleccionada.Columnas = Columnas;
            // El nombre debe venir bindeado directamente a SalaSeleccionada.Nombre en la vista,
            // o puedes tener una propiedad separada 'NombreSala' en el VM.
            // Asumiré que en la Vista el TextBox bindea a SalaSeleccionada.Nombre

            if (string.IsNullOrWhiteSpace(SalaSeleccionada.Nombre))
            {
                MessageBox.Show("Ingrese un nombre para la sala.");
                return;
            }

            (bool Exito, string Mensaje) resultado;

            if (SalaSeleccionada.Id == 0)
            {
                // CREAR
                resultado = await _servicioSala.CrearAsync(SalaSeleccionada);
            }
            else
            {
                // EDITAR
                resultado = await _servicioSala.EditarAsync(SalaSeleccionada);
            }

            MessageBox.Show(resultado.Mensaje);

            if (resultado.Exito)
            {
                await CargarSalas();
                Limpiar();
            }
        }

        [RelayCommand]
        private async Task Eliminar(Sala sala)
        {
            if (sala == null) return;

            var confirm = MessageBox.Show($"¿Eliminar la sala '{sala.Nombre}'?", "Confirmar", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                bool exito = await _servicioSala.EliminarAsync(sala.Id);
                if (exito)
                {
                    MessageBox.Show("Sala eliminada.");
                    await CargarSalas();
                    Limpiar();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar la sala. Verifique que no tenga funciones futuras.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        [RelayCommand]
        private void Limpiar()
        {
            // Reseteamos el objeto para modo "Crear"
            SalaSeleccionada = new Sala { Id = 0, Nombre = "", Estado = true };
            Filas = 0;
            Columnas = 0;
        }
    }
}