using System;
using System.Collections.Generic;
using System.Linq;
using CapaDatos;
using CapaEntidad;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio
{
    public class CN_Funcion
    {
        private readonly BDContexto _db;

        public CN_Funcion(string cadenaConexion)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BDContexto>();
            optionsBuilder.UseSqlServer(cadenaConexion);
            _db = new BDContexto(optionsBuilder.Options);
        }

        // Listar funciones incluyendo los datos de Pelicula y Sala (Include)
        public List<Funcion> Listar()
        {
            return _db.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.Estado == true)
                .ToList();
        }

        public string Guardar(Funcion obj)
        {
            // 1. Validar precio
            if (obj.PrecioTicket <= 0) return "El precio debe ser mayor a 0.";

            // 2. Calcular hora fin (Inicio + Duración de la peli + 15 min limpieza)
            var pelicula = _db.Peliculas.Find(obj.IdPelicula);
            if (pelicula == null) return "Película no encontrada.";

            obj.FechaHoraFin = obj.FechaHoraInicio.AddMinutes(pelicula.DuracionMinutos + 15);

            // 3. Validar que la sala no esté ocupada en ese horario
            if (ExisteSuperposicion(obj.IdSala, obj.FechaHoraInicio, obj.FechaHoraFin))
            {
                return "¡Conflicto! La sala está ocupada en ese horario.";
            }

            try
            {
                _db.Funciones.Add(obj);
                _db.SaveChanges();
                return "Función programada correctamente.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Lógica clave: Verifica si el nuevo horario choca con uno existente
        private bool ExisteSuperposicion(int idSala, DateTime inicio, DateTime fin)
        {
            return _db.Funciones
                .Any(f => f.IdSala == idSala
                       && f.Estado == true
                       && inicio < f.FechaHoraFin
                       && fin > f.FechaHoraInicio);
        }

        public void Eliminar(int id)
        {
            var funcion = _db.Funciones.Find(id);
            if (funcion != null)
            {
                funcion.Estado = false;
                _db.SaveChanges();
            }
        }
    }
}