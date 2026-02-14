using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
using CapaPresentacion_WPF.Vistas; 
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        private readonly ICN_Funcion _negocioFuncion;
        private readonly ICN_Ticket _negocioTicket;

        public int IdUsuarioActual { get; set; }

        public ObservableCollection<Funcion> ListaFunciones { get; set; } = new ObservableCollection<Funcion>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FinalizarVentaCommand))]
        private Funcion funcionSeleccionada;

        // Inyección de dependencias
        public VentasViewModel(ICN_Funcion negocioFuncion, ICN_Ticket negocioTicket)
        {
            _negocioFuncion = negocioFuncion;
            _negocioTicket = negocioTicket;

            _ = CargarDatosIniciales();
        }

        public async Task CargarDatosIniciales()
        {
            try
            {
                // Limpiamos visualmente antes de cargar
                // Usamos Application.Current.Dispatcher para asegurar que tocamos la UI desde el hilo correcto
                Application.Current.Dispatcher.Invoke(() => ListaFunciones.Clear());

                // 1. Ejecutar la consulta en hilo secundario
                var funcionesDesdeBD = await Task.Run(() =>
                {
                    // Nota: Asegúrate que tu CN_Funcion.Listar() use .AsNoTracking() si es posible
                    return _negocioFuncion.Listar().Where(f => f.Estado == true).ToList();
                });

                // 2. Actualizar la UI en el hilo principal
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var f in funcionesDesdeBD)
                    {
                        ListaFunciones.Add(f);
                    }
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al cargar la cartelera: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool PuedeFinalizarVenta() => FuncionSeleccionada != null;

        [RelayCommand(CanExecute = nameof(PuedeFinalizarVenta))]
        public void FinalizarVenta()
        {
            // 1. Validar que la función tenga una Sala asociada cargada correctamente
            if (FuncionSeleccionada.Sala == null)
            {
                MessageBox.Show("Error: No se pudo recuperar la información de la sala. Verifique que CN_Funcion incluya la Sala.", "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Obtener los asientos que YA están ocupados en la BD
            var ocupados = _negocioTicket.ObtenerAsientosOcupados(FuncionSeleccionada.Id);

            // 3. Preparar el ViewModel y la Ventana de Selección
            var vmSeleccion = new SeleccionAsientosViewModel(
                FuncionSeleccionada.Sala,
                FuncionSeleccionada.PrecioTicket,
                ocupados
            );

            var ventanaSeleccion = new SeleccionAsientosWindow
            {
                DataContext = vmSeleccion,
                Owner = Application.Current.MainWindow // Para que sea modal sobre la principal
            };

            // 4. Configurar qué pasa cuando el usuario confirma en la otra ventana
            vmSeleccion.OnConfirmar = (listaAsientosElegidos) =>
            {
                // Cerramos la ventana visual
                ventanaSeleccion.DialogResult = true;
                ventanaSeleccion.Close();

                // 5. Procesar la venta final
                ProcesarVenta(listaAsientosElegidos);
            };

            // 6. Mostrar la ventana de forma Modal (bloquea la de atrás)
            ventanaSeleccion.ShowDialog();
        }

        private void ProcesarVenta(List<string> asientos)
        {
            string mensaje;
            bool exito = _negocioTicket.RegistrarVenta(
            FuncionSeleccionada.Id,
            IdUsuarioActual, // <--- Usamos la variable que recibimos
            asientos,
            FuncionSeleccionada.PrecioTicket,
            out mensaje
        );

            if (exito)
            {
                MessageBox.Show($"¡Venta Exitosa!\nTickets generados: {asientos.Count}\nAsientos: {string.Join(", ", asientos)}", "Venta Completada", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reiniciamos la selección para obligar a recargar ocupados si quiere vender de nuevo
                FuncionSeleccionada = null;
            }
            else
            {
                MessageBox.Show($"Error en la venta:\n{mensaje}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}