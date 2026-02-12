using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        // 1. Mantenemos la Inyección de Dependencias intacta
        private readonly ICN_Funcion _negocioFuncion;
        private readonly ICN_Ticket _negocioTicket;

        public ObservableCollection<Funcion> ListaFunciones { get; set; } = new ObservableCollection<Funcion>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalVenta))]
        [NotifyCanExecuteChangedFor(nameof(FinalizarVentaCommand))] // MAGIA: Le avisa al botón que revise si debe habilitarse
        private Funcion funcionSeleccionada;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalVenta))]
        private int cantidadEntradas = 1;

        public decimal TotalVenta => (FuncionSeleccionada?.PrecioTicket ?? 0) * CantidadEntradas;

        // 2. Constructor con Inyección
        public VentasViewModel(ICN_Funcion negocioFuncion, ICN_Ticket negocioTicket)
        {
            _negocioFuncion = negocioFuncion;
            _negocioTicket = negocioTicket;
            CargarCartelera();
        }

        private void CargarCartelera()
        {
            ListaFunciones.Clear();
            var funciones = _negocioFuncion.Listar().Where(f => f.Estado == true);
            foreach (var f in funciones) ListaFunciones.Add(f);
        }

        // 3. Nuevos comandos para los botones de + y -
        [RelayCommand]
        public void AumentarEntradas() => CantidadEntradas++;

        [RelayCommand]
        public void DisminuirEntradas()
        {
            if (CantidadEntradas > 1) CantidadEntradas--;
        }

        // 4. Lógica que decide si el botón de vender está habilitado
        private bool PuedeFinalizarVenta() => FuncionSeleccionada != null;

        [RelayCommand(CanExecute = nameof(PuedeFinalizarVenta))]
        public void FinalizarVenta()
        {
            var nuevoTicket = new Ticket
            {
                IdFuncion = FuncionSeleccionada.Id,
                CantidadTickets = CantidadEntradas,
                MontoTotal = TotalVenta
            };

            string mensaje = _negocioTicket.RegistrarVenta(nuevoTicket);
            MessageBox.Show(mensaje);

            if (mensaje.Contains("éxito"))
            {
                FuncionSeleccionada = null;
                CantidadEntradas = 1;
            }
        }
    }
}