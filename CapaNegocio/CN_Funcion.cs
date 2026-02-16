using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Funcion : ICN_Funcion
    {
        private readonly BDContexto _db;

        public CN_Funcion(BDContexto db)
        {
            _db = db;
        }

        // 1. LISTAR POR FECHA (Optimizado para la vista diaria)
        public async Task<List<Funcion>> ListarPorFechaAsync(DateTime fecha)
        {
            try
            {
                return await _db.Funciones
                    .Include(f => f.Pelicula)
                    .Include(f => f.Sala)
                    .Where(f => f.Estado == true && f.FechaHoraInicio.Date == fecha.Date)
                    .OrderBy(f => f.FechaHoraInicio)
                    .AsNoTracking() // Mejora rendimiento solo lectura
                    .ToListAsync();
            }
            catch
            {
                return new List<Funcion>();
            }
        }

        // 2. LISTAR TODO (General)
        public async Task<List<Funcion>> ListarTodoAsync()
        {
            return await _db.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.Estado == true)
                .OrderByDescending(f => f.FechaHoraInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        // 3. CREAR FUNCIÓN (El núcleo de la lógica)
        public async Task<(bool Exito, string Mensaje)> CrearFuncionAsync(Funcion nuevaFuncion)
        {
            // A. Validaciones básicas de datos
            if (nuevaFuncion.PrecioTicket <= 0)
                return (false, "El precio del ticket debe ser mayor a 0.");

            if (string.IsNullOrEmpty(nuevaFuncion.Formato) || string.IsNullOrEmpty(nuevaFuncion.Idioma))
                return (false, "Debe especificar el Formato (2D/3D) y el Idioma.");

            try
            {
                // B. Obtener datos de la película para calcular duración
                var pelicula = await _db.Peliculas.FindAsync(nuevaFuncion.IdPelicula);
                if (pelicula == null)
                    return (false, "Error: La película seleccionada no existe.");

                // C. Calcular Hora de Finalización
                // Fórmula: Inicio + Duración Película + 15 min (Limpieza/Trailers)
                DateTime inicio = nuevaFuncion.FechaHoraInicio;
                DateTime fin = inicio.AddMinutes(pelicula.DuracionMinutos + 15);

                nuevaFuncion.FechaHoraFin = fin;

                // D. VALIDAR SUPERPOSICIÓN EN BD
                // Verificamos si existe alguna función en la MISMA SALA que choque con este rango.
                // Lógica de intersección: (StartA < EndB) y (EndA > StartB)
                bool salaOcupada = await _db.Funciones
                    .AnyAsync(f => f.IdSala == nuevaFuncion.IdSala
                                && f.Estado == true // Solo funciones activas
                                && f.Id != nuevaFuncion.Id // Excluirse a sí misma (por si editamos a futuro)
                                && inicio < f.FechaHoraFin
                                && fin > f.FechaHoraInicio);

                if (salaOcupada)
                {
                    // Buscamos cuál es la función que estorba para dar un mensaje detallado
                    var conflicto = await _db.Funciones
                        .Include(f => f.Pelicula)
                        .Where(f => f.IdSala == nuevaFuncion.IdSala
                               && f.Estado == true
                               && inicio < f.FechaHoraFin
                               && fin > f.FechaHoraInicio)
                        .FirstAsync();

                    return (false, $"Conflicto: La sala está ocupada por '{conflicto.Pelicula.Titulo}' de {conflicto.FechaHoraInicio:HH:mm} a {conflicto.FechaHoraFin:HH:mm}.");
                }

                // E. Guardar
                nuevaFuncion.Estado = true; // Aseguramos estado activo
                _db.Funciones.Add(nuevaFuncion);
                await _db.SaveChangesAsync();

                return (true, "Función programada correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error interno al guardar: {ex.Message}");
            }
        }

        // 4. ELIMINAR (Baja Lógica)
        public async Task<bool> EliminarFuncionAsync(int id)
        {
            try
            {
                var funcion = await _db.Funciones.FindAsync(id);
                if (funcion != null)
                {
                    funcion.Estado = false; // Soft Delete
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}