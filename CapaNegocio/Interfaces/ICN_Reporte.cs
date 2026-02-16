using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaces
{
    public class ResumenCajaDto
    {
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalGeneral { get; set; }
        public int CantidadTickets { get; set; }
        public List<DetalleVentaDto> UltimasVentas { get; set; }
    }

    public class DetalleVentaDto
    {
        public string Hora { get; set; }
        public string Pelicula { get; set; }
        public string MetodoPago { get; set; }
        public decimal Monto { get; set; }
    }

    public class EstadisticaPeliculaDto
    {
        public string Titulo { get; set; }
        public int TotalEntradas { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public interface ICN_Reporte
    {
        // Para el arqueo diario (Cierre de Caja)
        Task<ResumenCajaDto> ObtenerArqueoDiarioAsync(DateTime fecha);

        // Para gráficos y análisis
        Task<List<EstadisticaPeliculaDto>> ObtenerPeliculasMasVistasAsync(DateTime desde, DateTime hasta);
    }
}