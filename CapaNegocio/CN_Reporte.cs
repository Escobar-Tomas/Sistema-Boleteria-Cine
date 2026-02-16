using CapaDatos;
using CapaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte : ICN_Reporte
    {
        private readonly BDContexto _db;

        public CN_Reporte(BDContexto db)
        {
            _db = db;
        }

        public async Task<ResumenCajaDto> ObtenerArqueoDiarioAsync(DateTime fecha)
        {
            // Filtramos tickets de la fecha solicitada
            var ticketsDelDia = await _db.Tickets
                .Include(t => t.Funcion).ThenInclude(f => f.Pelicula)
                .Where(t => t.FechaVenta.Date == fecha.Date)
                .AsNoTracking()
                .ToListAsync();

            var resumen = new ResumenCajaDto
            {
                CantidadTickets = ticketsDelDia.Count,
                TotalGeneral = ticketsDelDia.Sum(t => t.Precio),

                // Agrupamos por texto (Asegúrate de usar los mismos strings que en VentasViewModel)
                TotalEfectivo = ticketsDelDia.Where(t => t.MetodoPago == "Efectivo").Sum(t => t.Precio),
                TotalTarjeta = ticketsDelDia.Where(t => t.MetodoPago.Contains("Tarjeta")).Sum(t => t.Precio),

                // Tomamos las últimas 20 ventas para mostrar en grilla
                UltimasVentas = ticketsDelDia
                    .OrderByDescending(t => t.FechaVenta)
                    .Take(20)
                    .Select(t => new DetalleVentaDto
                    {
                        Hora = t.FechaVenta.ToString("HH:mm"),
                        Pelicula = t.Funcion.Pelicula.Titulo,
                        MetodoPago = t.MetodoPago,
                        Monto = t.Precio
                    }).ToList()
            };

            return resumen;
        }

        public async Task<List<EstadisticaPeliculaDto>> ObtenerPeliculasMasVistasAsync(DateTime desde, DateTime hasta)
        {
            // Agrupar tickets por Película
            var stats = await _db.Tickets
                .Include(t => t.Funcion).ThenInclude(f => f.Pelicula)
                .Where(t => t.FechaVenta.Date >= desde.Date && t.FechaVenta.Date <= hasta.Date)
                .GroupBy(t => t.Funcion.Pelicula.Titulo)
                .Select(g => new EstadisticaPeliculaDto
                {
                    Titulo = g.Key,
                    TotalEntradas = g.Count(),
                    TotalIngresos = g.Sum(t => t.Precio)
                })
                .OrderByDescending(x => x.TotalIngresos)
                .ToListAsync();

            return stats;
        }
    }
}