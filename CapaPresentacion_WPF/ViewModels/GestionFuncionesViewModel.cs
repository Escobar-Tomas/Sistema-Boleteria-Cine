using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio;
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Linq;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionFuncionesViewModel : ObservableObject
    {
        // Negocio
        private CN_Funcion _negocioFunciones;
        private CN_Pelicula _negocioPeliculas; // Necesario para llenar el ComboBox
        private CN_Sala _negocioSalas;         // Necesario para llenar el ComboBox

        // Listas para la UI
        public ObservableCollection<Funcion> ListaFunciones { get; set; } = new ObservableCollection<Funcion>();
        public ObservableCollection<Pelicula> ListaPeliculas { get; set; } = new ObservableCollection<Pelicula>();
        public ObservableCollection<Sala> ListaSalas { get; set; } = new ObservableCollection<Sala>();

        // Propiedades del Formulario
        [ObservableProperty]
        private Pelicula peliculaSeleccionada;

        [ObservableProperty]
        private Sala salaSeleccionada;

        [ObservableProperty]
        private DateTime fechaEntrada = DateTime.Today; // Por defecto hoy

        [ObservableProperty]
        private string horaEntrada = "14:00"; // Formato texto simple para evitar complicaciones

        [ObservableProperty]
        private string precioEntrada = "5000"; // String para validar conversión luego

        public GestionFuncionesViewModel()
        {
            // Inicializamos todo con la cadena de conexión segura
            string cadena = App.CadenaConexion;
            string apiKey = App.TmdbApiKey;

            _negocioFunciones = new CN_Funcion(cadena);
            _negocioPeliculas = new CN_Pelicula(apiKey, cadena);
            _negocioSalas = new CN_Sala(cadena);

            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            // Carga Películas guardadas en BD
            var pelis = _negocioPeliculas.Listar();
            ListaPeliculas.Clear();
            if (pelis != null) foreach (var p in pelis) ListaPeliculas.Add(p);

            // Carga Salas activas
            var salas = _negocioSalas.Listar();
            ListaSalas.Clear();
            if (salas != null) foreach (var s in salas) ListaSalas.Add(s);

            CargarFunciones();
        }

        private void CargarFunciones()
        {
            ListaFunciones.Clear();
            var funciones = _negocioFunciones.Listar();
            // Ordenamos por fecha para que se vea bonito
            foreach (var f in funciones.OrderByDescending(x => x.FechaHoraInicio))
                ListaFunciones.Add(f);
        }

        [RelayCommand]
        public void Guardar()
        {
            // 1. Validaciones básicas de UI
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

            // 2. Intentar convertir la hora (texto) a DateTime
            if (!TimeSpan.TryParse(HoraEntrada, out TimeSpan horaFinal))
            {
                MessageBox.Show("Formato de hora inválido. Usa HH:mm (ej: 18:30)");
                return;
            }

            // 3. Crear el objeto Funcion combinando Fecha + Hora
            DateTime fechaInicioCombinada = FechaEntrada.Date + horaFinal;

            var nuevaFuncion = new Funcion
            {
                IdPelicula = PeliculaSeleccionada.Id,
                IdSala = SalaSeleccionada.Id,
                FechaHoraInicio = fechaInicioCombinada,
                PrecioTicket = precioFinal,
                Estado = true
            };

            // 4. Enviar a Negocio (Allí se valida el choque de horarios)
            string resultado = _negocioFunciones.Guardar(nuevaFuncion);
            MessageBox.Show(resultado);

            if (resultado.Contains("correctamente"))
            {
                CargarFunciones(); // Refrescar tabla
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