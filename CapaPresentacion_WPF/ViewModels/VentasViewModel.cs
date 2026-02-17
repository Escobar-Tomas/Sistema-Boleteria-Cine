using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
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
        private string metodoPago = "Efectivo";

        public List<string> ListaMetodosPago { get; } = new List<string>
        {
            "Efectivo",
            "Tarjeta Débito",
            "Tarjeta Crédito",
            "Mercado Pago"
        };

        public ObservableCollection<Funcion> FuncionesDelDia { get; set; } = new ObservableCollection<Funcion>();

        [ObservableProperty]
        private DateTime fechaVenta = DateTime.Today;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(IniciarSeleccionCommand))]
        private Funcion funcionSeleccionada;

        // ---  PROPIEDADES PARA EL DETALLE DE VENTA ---

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmarVentaCommand))]
        private string _textoAsientosSeleccionados = "Ninguno";

        [ObservableProperty]
        private decimal _totalVenta = 0;

        // Guardamos los asientos en memoria temporalmente hasta confirmar
        private List<string> _asientosTemporales = new List<string>();


        public VentasViewModel(ICN_Funcion negocioFuncion, ICN_Ticket negocioTicket)
        {
            _negocioFuncion = negocioFuncion;
            _negocioTicket = negocioTicket;
            _ = CargarFunciones();
        }

        [RelayCommand]
        public async Task CargarFunciones()
        {
            try
            {
                var lista = await _negocioFuncion.ListarPorFechaAsync(FechaVenta);
                FuncionesDelDia.Clear();
                foreach (var f in lista) FuncionesDelDia.Add(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar cartelera: {ex.Message}");
            }
        }

        [RelayCommand]
        public void SeleccionarFuncion(Funcion funcion)
        {
            FuncionSeleccionada = funcion;
            // Al cambiar de función, limpiamos la selección anterior
            LimpiarSeleccion();
        }

        private bool PuedeIniciarSeleccion() => FuncionSeleccionada != null;

        // PASO 1: Abrir la ventana solo para seleccionar
        [RelayCommand(CanExecute = nameof(PuedeIniciarSeleccion))]
        public void IniciarSeleccion()
        {
            if (FuncionSeleccionada == null) return;

            if (FuncionSeleccionada.Sala == null)
            {
                MessageBox.Show("Error: La función no tiene Sala asignada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var ocupados = _negocioTicket.ObtenerAsientosOcupados(FuncionSeleccionada.Id);
                var vmSeleccion = new SeleccionAsientosViewModel(FuncionSeleccionada.Sala, FuncionSeleccionada.PrecioTicket, ocupados);
                var ventana = new SeleccionAsientosWindow { DataContext = vmSeleccion, Owner = Application.Current.MainWindow };

                vmSeleccion.OnConfirmar = (asientos) => {
                    ventana.DialogResult = true;
                    ventana.Close();

                    ActualizarResumenVenta(asientos);
                };

                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir selección: {ex.Message}");
            }
        }

        private void ActualizarResumenVenta(List<string> asientos)
        {
            _asientosTemporales = asientos;

            if (asientos != null && asientos.Any())
            {
                TextoAsientosSeleccionados = string.Join(", ", asientos);
                TotalVenta = asientos.Count * FuncionSeleccionada.PrecioTicket;
            }
            else
            {
                LimpiarSeleccion();
            }
            // Notificamos al botón de confirmar que ya puede activarse
            ConfirmarVentaCommand.NotifyCanExecuteChanged();
        }

        private bool PuedeConfirmarVenta() => _asientosTemporales != null && _asientosTemporales.Any();

        // El botón final que guarda en la BD
        [RelayCommand(CanExecute = nameof(PuedeConfirmarVenta))]
        public void ConfirmarVenta()
        {
            if (!PuedeConfirmarVenta()) return;

            string mensaje;
            bool exito = _negocioTicket.RegistrarVenta(
                FuncionSeleccionada.Id,
                IdUsuarioActual,
                _asientosTemporales,
                FuncionSeleccionada.PrecioTicket,
                MetodoPago, // Usamos la propiedad mayúscula generada por ObservableProperty
                out mensaje
            );

            if (exito)
            {
                MessageBox.Show($"¡Venta Exitosa!\nTickets: {_asientosTemporales.Count}\nTotal: ${TotalVenta}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarSeleccion();
                _ = CargarFunciones(); // Recargar para actualizar disponibilidad si fuera necesario
            }
            else
            {
                MessageBox.Show($"Error al registrar venta: {mensaje}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarSeleccion()
        {
            _asientosTemporales.Clear();
            TextoAsientosSeleccionados = "Ninguno";
            TotalVenta = 0;
            MetodoPago = "Efectivo";
            ConfirmarVentaCommand.NotifyCanExecuteChanged();
        }
    }
}