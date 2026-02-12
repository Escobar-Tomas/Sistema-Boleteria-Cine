using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using CapaNegocio.ModelosAPI;
using CapaNegocio.Servicios;
using Microsoft.EntityFrameworkCore; // ¡Vital para ToListAsync!

namespace CapaNegocio
{
    public class CN_Pelicula : ICN_Pelicula
    {
        private readonly BDContexto _db;
        private readonly ITmdbService _tmdbService;

        public CN_Pelicula(BDContexto db, ITmdbService tmdbService)
        {
            _db = db;
            _tmdbService = tmdbService;
        }

        // 1. BUSCAR EN API (Ya era async, solo ajustamos nombre)
        public async Task<List<PeliculaBusqueda>> BuscarPeliculasEnTMDBAsync(string nombre)
        {
            return await _tmdbService.BuscarPeliculasAsync(nombre);
        }

        // 2. GUARDAR (Usamos SaveChangesAsync y AnyAsync)
        public async Task<string> GuardarPeliculaDesdeApiAsync(int idTmdb)
        {
            // Verificamos existencia de forma asíncrona
            bool existe = await _db.Peliculas.AnyAsync(p => p.TmdbId == idTmdb);
            if (existe) return "La película ya existe en la base de datos.";

            DetallePelicula detalle = await _tmdbService.ObtenerDetallePeliculaAsync(idTmdb);
            if (detalle == null) return "Error al obtener datos de TMDB.";

            Pelicula nuevaPelicula = new Pelicula
            {
                Titulo = detalle.Titulo,
                Sinopsis = detalle.Sinopsis,
                DuracionMinutos = detalle.DuracionMinutos,
                PosterUrl = "https://image.tmdb.org/t/p/w500" + detalle.PosterPath,
                TmdbId = detalle.Id,
                EstaEnCartelera = true,
                Genero = detalle.Generos?.FirstOrDefault()?.Nombre ?? "General",
                Director = detalle.Creditos?.Equipo?.FirstOrDefault(p => p.Trabajo == "Director")?.Nombre ?? "Desconocido"
            };

            try
            {
                _db.Peliculas.Add(nuevaPelicula);
                await _db.SaveChangesAsync(); // Guardado asíncrono (no bloquea UI)
                return $"Película '{nuevaPelicula.Titulo}' guardada correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error al guardar BD: {ex.Message}";
            }
        }

        // 3. LISTAR LOCAL (Usamos ToListAsync)
        public async Task<List<Pelicula>> ListarAsync()
        {
            try
            {
                // Ordenamos y materializamos la lista en un hilo secundario
                return await _db.Peliculas.OrderByDescending(p => p.Id).ToListAsync();
            }
            catch
            {
                return new List<Pelicula>();
            }
        }

        public async Task EditarAsync(Pelicula pelicula)
        {
            // Validaciones básicas antes de editar
            if (string.IsNullOrEmpty(pelicula.Titulo)) throw new Exception("El título es obligatorio");

            _db.Peliculas.Update(pelicula);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EliminarAsync(int id)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p != null)
            {
                _db.Peliculas.Remove(p);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}