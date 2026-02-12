using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaEntidad;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class SeleccionAsientosViewModel : ObservableObject
    {
        private readonly Sala _sala;

        // Propiedades para dimensionar el UniformGrid
        public int Filas => _sala.Filas;
        public int Columnas => _sala.Columnas;

        public ObservableCollection<AsientoItem> ListaAsientos { get; set; } = new ObservableCollection<AsientoItem>();

        [ObservableProperty]
        private string _resumenSeleccion = "0 seleccionados";

        [ObservableProperty]
        private decimal _total = 0;
        private decimal _precioUnitario;

        public System.Action<List<string>>? OnConfirmar { get; set; }

        public SeleccionAsientosViewModel(Sala sala, decimal precio, List<string> ocupados)
        {
            _sala = sala;
            _precioUnitario = precio;
            GenerarMapa(ocupados);
        }

        private void GenerarMapa(List<string> ocupados)
        {
            ListaAsientos.Clear();
            char letra = 'A';

            for (int i = 0; i < _sala.Filas; i++)
            {
                for (int j = 1; j <= _sala.Columnas; j++)
                {
                    string codigo = $"{letra}{j}";
                    ListaAsientos.Add(new AsientoItem(codigo, ocupados.Contains(codigo), this));
                }
                letra++;
            }
        }

        public void Recalcular()
        {
            var seleccionados = ListaAsientos.Where(x => x.Seleccionado).ToList();
            ResumenSeleccion = $"{seleccionados.Count} Asientos ({string.Join(",", seleccionados.Select(x => x.Codigo))})";
            Total = seleccionados.Count * _precioUnitario;
        }

        [RelayCommand]
        public void Confirmar()
        {
            var seleccion = ListaAsientos.Where(x => x.Seleccionado).Select(x => x.Codigo).ToList();
            if (seleccion.Count > 0) OnConfirmar?.Invoke(seleccion);
        }
    }

    public partial class AsientoItem : ObservableObject
    {
        public string Codigo { get; }
        public bool Ocupado { get; }
        private readonly SeleccionAsientosViewModel _padre;

        [ObservableProperty]
        private bool _seleccionado;

        [ObservableProperty]
        private Brush _color;

        public AsientoItem(string codigo, bool ocupado, SeleccionAsientosViewModel padre)
        {
            Codigo = codigo;
            Ocupado = ocupado;
            _padre = padre;
            Color = Ocupado ? Brushes.Red : Brushes.Gray; // Rojo si ocupado, Gris si libre
        }

        [RelayCommand]
        public void Click()
        {
            if (Ocupado) return;
            Seleccionado = !Seleccionado;
            Color = Seleccionado ? Brushes.Green : Brushes.Gray; // Verde si selecciono
            _padre.Recalcular();
        }
    }
}