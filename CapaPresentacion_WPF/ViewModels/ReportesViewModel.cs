using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CapaNegocio.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace CapaPresentacion_WPF.ViewModels
{
    public partial class ReportesViewModel : ObservableObject
    {
        private readonly ICN_Reporte _negocioReporte;

        // --- ARQUEO DE CAJA ---
        [ObservableProperty] private DateTime fechaArqueo = DateTime.Today;
        [ObservableProperty] private ResumenCajaDto resumenCaja;

        // --- ESTADÍSTICAS ---
        [ObservableProperty] private DateTime fechaDesde = DateTime.Today.AddDays(-30); // Último mes
        [ObservableProperty] private DateTime fechaHasta = DateTime.Today;
        public ObservableCollection<EstadisticaPeliculaDto> TopPeliculas { get; set; } = new();

        public ReportesViewModel(ICN_Reporte negocioReporte)
        {
            _negocioReporte = negocioReporte;
            _ = CargarArqueo(); // Carga inicial
        }

        [RelayCommand]
        public async Task CargarArqueo()
        {
            try
            {
                ResumenCaja = await _negocioReporte.ObtenerArqueoDiarioAsync(fechaArqueo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar caja: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task CargarEstadisticas()
        {
            try
            {
                var lista = await _negocioReporte.ObtenerPeliculasMasVistasAsync(fechaDesde, fechaHasta);
                TopPeliculas.Clear();
                foreach (var item in lista) TopPeliculas.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estadísticas: {ex.Message}");
            }
        }
    }
}