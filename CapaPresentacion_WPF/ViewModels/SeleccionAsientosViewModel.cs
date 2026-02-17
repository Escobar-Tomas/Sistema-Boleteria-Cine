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
        private readonly decimal _precioUnitario;

        public int Filas => _sala.Filas;
        public int Columnas => _sala.Columnas;
        public string TituloSala => _sala.Nombre;

        public ObservableCollection<AsientoItem> ListaAsientos { get; set; } = new ObservableCollection<AsientoItem>();

        [ObservableProperty]
        private string _resumenSeleccion = "0 seleccionados";

        [ObservableProperty]
        private decimal _total = 0;

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
            for (int i = 0; i < _sala.Filas; i++)
            {
                char letraFila = (char)('A' + i);
                for (int j = 1; j <= _sala.Columnas; j++)
                {
                    string codigo = $"{letraFila}{j}";
                    bool estaOcupado = ocupados != null && ocupados.Contains(codigo);
                    ListaAsientos.Add(new AsientoItem(codigo, estaOcupado, this));
                }
            }
        }

        public void RecalcularTotal()
        {
            var seleccionados = ListaAsientos.Where(x => x.IsSelected).ToList();
            int cantidad = seleccionados.Count;

            Total = cantidad * _precioUnitario;

            if (cantidad == 0)
                ResumenSeleccion = "Ningún asiento seleccionado";
            else
                ResumenSeleccion = $"{cantidad} asientos: {string.Join(", ", seleccionados.Select(x => x.Codigo))}";
        }

        [RelayCommand]
        public void Confirmar()
        {
            var seleccion = ListaAsientos.Where(x => x.IsSelected).Select(x => x.Codigo).ToList();
            if (seleccion.Count > 0)
            {
                OnConfirmar?.Invoke(seleccion);
            }
        }
    }

    // 'partial' para evitar conflictos con el generador MVVM
    public partial class AsientoItem : ObservableObject
    {
        public string Codigo { get; }
        public bool IsOccupied { get; }
        private readonly SeleccionAsientosViewModel _padre;

        // Uso explícito de 'System.Windows.Media.Color' para evitar ambigüedad
        private static readonly Brush ColorLibre = new SolidColorBrush(System.Windows.Media.Color.FromRgb(52, 152, 219));
        private static readonly Brush ColorOcupado = new SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60));
        private static readonly Brush ColorSeleccionado = new SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 204, 113));

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    ActualizarColor();
                    _padre.RecalcularTotal();
                }
            }
        }

        private Brush _backgroundColor;
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private Brush _foregroundColor;
        public Brush ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

        public RelayCommand ToggleSelectionCommand { get; }

        public AsientoItem(string codigo, bool ocupado, SeleccionAsientosViewModel padre)
        {
            Codigo = codigo;
            IsOccupied = ocupado;
            _padre = padre;

            ToggleSelectionCommand = new RelayCommand(ToggleSelection);

            // Estado inicial
            BackgroundColor = IsOccupied ? ColorOcupado : ColorLibre;
            ForegroundColor = Brushes.White;
        }

        private void ToggleSelection()
        {
            if (IsOccupied) return;
            IsSelected = !IsSelected;
        }

        private void ActualizarColor()
        {
            BackgroundColor = IsSelected ? ColorSeleccionado : ColorLibre;
        }
    }
}