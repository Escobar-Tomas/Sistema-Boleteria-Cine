using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
// Asegúrate de tener este using para la ventana de asientos
// using CapaPresentacion_WPF.Vistas; 
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using CapaPresentacion_WPF.Vistas;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        private readonly ICN_Funcion _negocioFuncion;
        private readonly ICN_Ticket _negocioTicket;

        // Propiedades de Usuario
        public int IdUsuarioActual { get; set; } = 1;

        [ObservableProperty]
        private string metodoPago = "Efectivo"; // Valor por defecto

        public List<string> ListaMetodosPago { get; } = new List<string>
        {
            "Efectivo",
            "Tarjeta Débito",
            "Tarjeta Crédito",
            "Mercado Pago"
        };

        // --- COLECCIONES ---
        // Renombramos a FuncionesDelDia para ser más específicos
        public ObservableCollection<Funcion> FuncionesDelDia { get; set; } = new ObservableCollection<Funcion>();

        // --- FILTROS ---
        [ObservableProperty]
        private DateTime fechaVenta = DateTime.Today;

        // --- SELECCIÓN ---
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(IniciarVentaCommand))]
        private Funcion funcionSeleccionada;

        public VentasViewModel(ICN_Funcion negocioFuncion, ICN_Ticket negocioTicket)
        {
            _negocioFuncion = negocioFuncion;
            _negocioTicket = negocioTicket;

            // Iniciamos carga asíncrona
            _ = CargarFunciones();
        }

        [RelayCommand]
        public async Task CargarFunciones()
        {
            try
            {
                // CORRECCIÓN PRINCIPAL: Usamos el método asíncrono y filtramos por fecha
                var lista = await _negocioFuncion.ListarPorFechaAsync(fechaVenta);

                FuncionesDelDia.Clear();
                foreach (var f in lista)
                {
                    // Opcional: Filtrar funciones que ya pasaron hace mucho (ej: más de 2 horas)
                    // if (f.FechaHoraInicio > DateTime.Now.AddHours(-2)) 
                    FuncionesDelDia.Add(f);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar cartelera: {ex.Message}");
            }
        }

        // Método auxiliar para seleccionar desde la UI (Botón en tarjeta)
        [RelayCommand]
        public void SeleccionarFuncion(Funcion funcion)
        {
            FuncionSeleccionada = funcion;
        }

        private bool PuedeIniciarVenta() => FuncionSeleccionada != null;

        [RelayCommand(CanExecute = nameof(PuedeIniciarVenta))]
        public void IniciarVenta()
        {
            if (FuncionSeleccionada == null) return;

            // Validar Sala
            if (FuncionSeleccionada.Sala == null)
            {
                MessageBox.Show("Error de datos: La función no tiene Sala asignada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 1. Obtener asientos ocupados (Asumimos que Ticket sigue siendo síncrono por ahora)
                var ocupados = _negocioTicket.ObtenerAsientosOcupados(FuncionSeleccionada.Id);

                // 2. Instanciar VM y Ventana de Asientos (Tu lógica original)
                // Nota: Asegúrate de tener referenciada la clase SeleccionAsientosViewModel
                /* Si te da error aquí, verifica que SeleccionAsientosViewModel reciba los parámetros correctos.
                   Asumo constructor: (Sala sala, decimal precio, List<string> ocupados)
                */
                var vmSeleccion = new SeleccionAsientosViewModel(FuncionSeleccionada.Sala, FuncionSeleccionada.PrecioTicket, ocupados);

                var ventana = new SeleccionAsientosWindow { DataContext = vmSeleccion, Owner = Application.Current.MainWindow };

                vmSeleccion.OnConfirmar = (asientos) => {
                    ventana.DialogResult = true;
                    ventana.Close();
                    ProcesarVenta(asientos);
                 };

                ventana.ShowDialog();

                // COMO NO TENGO TU CLASE DE ASIENTOS, DEJO ESTE PLACEHOLDER:
                // MessageBox.Show($"Aquí se abriría la selección de asientos para '{FuncionSeleccionada.Pelicula.Titulo}'.\nImplementa la llamada a tu ventana SeleccionAsientosWindow aquí.", "Flujo de Venta");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar venta: {ex.Message}");
            }
        }

        public void ProcesarVenta(List<string> asientos)
        {
            if (asientos == null || !asientos.Any()) return;

            string mensaje;
            bool exito = _negocioTicket.RegistrarVenta(
                FuncionSeleccionada.Id,
                IdUsuarioActual,
                asientos,
                FuncionSeleccionada.PrecioTicket,
                metodoPago,
                out mensaje
            );

            if (exito)
            {
                MessageBox.Show($"¡Venta Exitosa!\nTickets: {asientos.Count}\nTotal: ${asientos.Count * FuncionSeleccionada.PrecioTicket}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                FuncionSeleccionada = null; // Limpiar selección
                metodoPago = "Efectivo";
            }
            else
            {
                MessageBox.Show($"Error al registrar venta: {mensaje}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}