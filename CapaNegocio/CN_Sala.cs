using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para OrderBy
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Sala : ICN_Sala
    {
        private readonly BDContexto _db;

        public CN_Sala(BDContexto db)
        {
            _db = db;
        }

        public async Task<List<Sala>> ListarAsync()
        {
            // Retornamos todas las salas activas ordenadas por nombre
            return await _db.Salas
                .Where(s => s.Estado == true)
                .OrderBy(s => s.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(bool Exito, string Mensaje)> CrearAsync(Sala sala)
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(sala.Nombre))
                return (false, "El nombre de la sala es obligatorio.");

            if (sala.Filas <= 0 || sala.Columnas <= 0)
                return (false, "Las filas y columnas deben ser mayores a 0.");

            // 2. Validación de duplicados (Async)
            bool existe = await _db.Salas.AnyAsync(s => s.Nombre.ToLower() == sala.Nombre.ToLower() && s.Estado == true);
            if (existe)
                return (false, "Ya existe una sala activa con ese nombre.");

            try
            {
                // Calcular capacidad total
                sala.Capacidad = sala.Filas * sala.Columnas;
                sala.Estado = true;

                _db.Salas.Add(sala);
                await _db.SaveChangesAsync();
                return (true, "Sala creada correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error interno al guardar: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> EditarAsync(Sala sala)
        {
            var salaDb = await _db.Salas
                .Include(s => s.Funciones) // Traemos funciones para validar uso
                .FirstOrDefaultAsync(s => s.Id == sala.Id);

            if (salaDb == null) return (false, "La sala no existe.");

            // Validar duplicado de nombre (excluyendo la actual)
            if (salaDb.Nombre.ToLower() != sala.Nombre.ToLower())
            {
                bool existe = await _db.Salas.AnyAsync(s => s.Nombre.ToLower() == sala.Nombre.ToLower() && s.Id != sala.Id && s.Estado == true);
                if (existe) return (false, "Ya existe otra sala con ese nombre.");
            }

            // VALIDACIÓN CRÍTICA: No cambiar tamaño si tiene funciones futuras
            bool tieneFuncionesFuturas = salaDb.Funciones.Any(f => f.FechaHoraInicio > DateTime.Now && f.Estado == true);

            // Si intenta cambiar filas/columnas y tiene funciones pendientes...
            if ((salaDb.Filas != sala.Filas || salaDb.Columnas != sala.Columnas) && tieneFuncionesFuturas)
            {
                return (false, "No se puede modificar el tamaño de la sala porque tiene funciones programadas a futuro. Cancele las funciones primero.");
            }

            try
            {
                salaDb.Nombre = sala.Nombre;
                salaDb.Filas = sala.Filas;
                salaDb.Columnas = sala.Columnas;
                salaDb.Capacidad = sala.Filas * sala.Columnas; // Recalcular siempre

                await _db.SaveChangesAsync();
                return (true, "Sala actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al editar: {ex.Message}");
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var sala = await _db.Salas.FindAsync(id);
            if (sala != null)
            {
                // Validación opcional: No borrar si tiene funciones futuras
                bool tieneFunciones = await _db.Funciones.AnyAsync(f => f.IdSala == id && f.FechaHoraInicio > DateTime.Now && f.Estado == true);

                if (tieneFunciones) return false; // Bloquear borrado

                sala.Estado = false; // Baja lógica
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}