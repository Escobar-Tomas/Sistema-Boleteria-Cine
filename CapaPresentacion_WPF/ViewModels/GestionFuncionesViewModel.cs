using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces; // IMPORTANTE: Usamos las interfaces
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Linq;
using System.Threading.Tasks; // Necesario para async/await

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionFuncionesViewModel : ObservableObject
    {
        // 1. Usamos INTERFACES (Inyección de Dependencias)
        private readonly ICN_Funcion _negocioFunciones;
        private readonly ICN_Pelicula _negocioPeliculas;
        private readonly ICN_Sala _negocioSalas;

        // Listas para la UI
        public ObservableCollection<Funcion> ListaFunciones { get; set; } = new ObservableCollection<Funcion>();
        public ObservableCollection<Pelicula> ListaPeliculas { get; set; } = new ObservableCollection<Pelicula>();
        public ObservableCollection<Sala> ListaSalas { get; set; } = new ObservableCollection<Sala>();

        // Propiedades del Formulario
        [ObservableProperty] private Pelicula peliculaSeleccionada;
        [ObservableProperty] private Sala salaSeleccionada;
        [ObservableProperty] private DateTime fechaEntrada = DateTime.Today;
        [ObservableProperty] private string horaEntrada = "14:00";
        [ObservableProperty] private string precioEntrada = "5000";

        // 2. CONSTRUCTOR CON INYECCIÓN (Nada de 'new')
        public GestionFuncionesViewModel(ICN_Funcion negocioFunciones, ICN_Pelicula negocioPeliculas, ICN_Sala negocioSalas)
        {
            _negocioFunciones = negocioFunciones;
            _negocioPeliculas = negocioPeliculas;
            _negocioSalas = negocioSalas;

            // Llamamos a la carga de datos de forma segura para el constructor
            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            // A. CARGA ASÍNCRONA DE PELÍCULAS (Lo nuevo)
            var listaPelis = await _negocioPeliculas.ListarAsync();
            ListaPeliculas.Clear();
            foreach (var p in listaPelis) ListaPeliculas.Add(p);

            // B. CARGA SÍNCRONA DE SALAS Y FUNCIONES (Aún no las hemos migrado a async)
            // Nota: Cuando migremos Sala y Función, añadiremos 'await' aquí también.
            CargarSalas();
            CargarFunciones();
        }

        private void CargarSalas()
        {
            var salas = _negocioSalas.Listar();
            ListaSalas.Clear();
            foreach (var s in salas) ListaSalas.Add(s);
        }

        private void CargarFunciones()
        {
            var funciones = _negocioFunciones.Listar();
            ListaFunciones.Clear();
            foreach (var f in funciones.OrderByDescending(x => x.FechaHoraInicio))
                ListaFunciones.Add(f);
        }

        [RelayCommand]
        public void Guardar()
        {
            if (PeliculaSeleccionada == null || SalaSeleccionada == null)
            {
                MessageBox.Show("Debes seleccionar una Película y una Sala.");
                return;
            }

            if (!decimal.TryParse(PrecioEntrada, out decimal precioFinal))
            {
                MessageBox.Show("El precio debe ser un número válido.");
                return;
            }

            if (!TimeSpan.TryParse(HoraEntrada, out TimeSpan horaFinal))
            {
                MessageBox.Show("Formato de hora inválido. Usa HH:mm (ej: 18:30)");
                return;
            }

            DateTime fechaInicioCombinada = FechaEntrada.Date + horaFinal;

            var nuevaFuncion = new Funcion
            {
                IdPelicula = PeliculaSeleccionada.Id,
                IdSala = SalaSeleccionada.Id,
                FechaHoraInicio = fechaInicioCombinada,
                PrecioTicket = precioFinal,
                Estado = true
            };

            string resultado = _negocioFunciones.Guardar(nuevaFuncion);
            MessageBox.Show(resultado);

            if (resultado.Contains("correctamente"))
            {
                CargarFunciones();
            }
        }

        [RelayCommand]
        public void Eliminar(Funcion funcion)
        {
            if (funcion == null) return;

            if (MessageBox.Show($"¿Eliminar función de {funcion.Pelicula.Titulo}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _negocioFunciones.Eliminar(funcion.Id);
                CargarFunciones();
            }
        }
    }
}