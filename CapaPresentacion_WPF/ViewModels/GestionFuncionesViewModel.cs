using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaEntidad;
using CapaNegocio.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Collections.Generic; // Para List<string>

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class GestionFuncionesViewModel : ObservableObject
    {
        private readonly ICN_Funcion _cnFuncion;
        private readonly ICN_Pelicula _cnPelicula;
        private readonly ICN_Sala _cnSala;

        // --- LISTAS ---
        public ObservableCollection<Funcion> ListaFunciones { get; set; } = new();
        public ObservableCollection<Pelicula> ListaPeliculas { get; set; } = new();
        public ObservableCollection<Sala> ListaSalas { get; set; } = new();

        // Listas estáticas para los ComboBox de Formato e Idioma
        public List<string> ListaFormatos { get; } = new() { "2D", "3D", "IMAX", "4DX" };
        public List<string> ListaIdiomas { get; } = new() { "Doblada", "Subtitulada", "Original" };

        // --- CAMPOS SELECCIONADOS ---
        [ObservableProperty] private Pelicula peliculaSeleccionada;
        [ObservableProperty] private Sala salaSeleccionada;

        [ObservableProperty] private string formatoSeleccionado = "2D";
        [ObservableProperty] private string idiomaSeleccionado = "Doblada";

        [ObservableProperty] private DateTime fechaEntrada = DateTime.Today; // Solo fecha
        [ObservableProperty] private string horaEntrada = "18:00";           // Texto HH:mm
        [ObservableProperty] private string precioEntrada = "5000";          // String para facilitar input

        // --- FILTROS ---
        [ObservableProperty] private DateTime fechaFiltro = DateTime.Today;

        public GestionFuncionesViewModel(ICN_Funcion cnFuncion, ICN_Pelicula cnPelicula, ICN_Sala cnSala)
        {
            _cnFuncion = cnFuncion;
            _cnPelicula = cnPelicula;
            _cnSala = cnSala;

            _ = CargarDatosIniciales();
        }

        private async Task CargarDatosIniciales()
        {
            // Cargar Películas
            var pelis = await _cnPelicula.ListarAsync();
            ListaPeliculas.Clear();
            foreach (var p in pelis) ListaPeliculas.Add(p);

            // Cargar Salas
            var salas = await _cnSala.ListarAsync();
            ListaSalas.Clear();
            foreach (var s in salas) ListaSalas.Add(s);
            
            // 3. Cargar Funciones del día
            await CargarFunciones();
        }

        [RelayCommand]
        public async Task CargarFunciones()
        {
            var lista = await _cnFuncion.ListarPorFechaAsync(fechaFiltro);
            ListaFunciones.Clear();
            foreach (var f in lista) ListaFunciones.Add(f);
        }

        [RelayCommand]
        public async Task CrearFuncion()
        {
            // Validaciones de UI
            if (PeliculaSeleccionada == null || SalaSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar Película y Sala.");
                return;
            }

            if (!TimeSpan.TryParse(HoraEntrada, out TimeSpan hora))
            {
                MessageBox.Show("La hora debe ser válida (HH:mm). Ej: 14:30");
                return;
            }

            if (!decimal.TryParse(PrecioEntrada, out decimal precio))
            {
                MessageBox.Show("El precio debe ser numérico.");
                return;
            }

            // Construir Fecha Completa
            DateTime inicio = FechaEntrada.Date + hora;

            // Validar futuro
            if (inicio < DateTime.Now)
            {
                MessageBox.Show("No puedes crear funciones en el pasado.");
                return;
            }

            var nueva = new Funcion
            {
                IdPelicula = PeliculaSeleccionada.Id,
                IdSala = SalaSeleccionada.Id,
                FechaHoraInicio = inicio,
                PrecioTicket = precio,
                Formato = formatoSeleccionado,
                Idioma = idiomaSeleccionado,
                Estado = true
            };

            // Llamada al Negocio (Aquí ocurre la validación de superposición)
            var resultado = await _cnFuncion.CrearFuncionAsync(nueva);

            if (resultado.Exito)
            {
                MessageBox.Show(resultado.Mensaje);
                // Si la fecha creada coincide con el filtro actual, recargamos la lista
                if (FechaEntrada.Date == FechaFiltro.Date)
                    await CargarFunciones();
            }
            else
            {
                MessageBox.Show(resultado.Mensaje, "Error de Programación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        public async Task EliminarFuncion(Funcion f)
        {
            if (f == null) return;

            if (MessageBox.Show($"¿Eliminar la función de las {f.FechaHoraInicio:HH:mm}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool exito = await _cnFuncion.EliminarFuncionAsync(f.Id);
                if (exito) await CargarFunciones();
            }
        }
    }
}