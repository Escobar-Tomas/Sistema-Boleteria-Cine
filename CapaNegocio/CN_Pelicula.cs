using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using CapaNegocio.ModelosAPI;
using CapaNegocio.Servicios;
using Microsoft.EntityFrameworkCore;

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

        // BUSCAR EN API
        public async Task<List<PeliculaBusqueda>> BuscarPeliculasEnTMDBAsync(string nombre)
        {
            return await _tmdbService.BuscarPeliculasAsync(nombre);
        }

        // GUARDAR
        public async Task<string> GuardarPeliculaDesdeApiAsync(int idTmdb)
        {
            // 1. Verificamos existencia (Correcto)
            bool existe = await _db.Peliculas.AnyAsync(p => p.TmdbId == idTmdb);
            if (existe) return "La película ya existe en la base de datos.";

            // 2. Obtenemos detalle (Asegúrate de que ObtenerDetallePeliculaAsync incluya &append_to_response=credits,release_dates)
            DetallePelicula detalle = await _tmdbService.ObtenerDetallePeliculaAsync(idTmdb);

            if (detalle == null) return "Error al obtener datos de TMDB. Aqui ta";

            // 3. Lógica para extraer la Clasificación (NUEVO)
            string clasificacion = "S/C"; // Valor por defecto "Sin Calificar"

            if (detalle.ReleaseDates?.Results != null)
            {
                // Buscamos preferentemente la certificación de USA ("US") o Argentina ("AR")
                // Puedes cambiar "AR" por tu país si lo prefieres
                var releaseInfo = detalle.ReleaseDates.Results
                                    .FirstOrDefault(r => r.CountryCode == "US" || r.CountryCode == "AR");

                if (releaseInfo != null && releaseInfo.ReleaseDates.Any())
                {
                    // Tomamos la primera certificación válida que encontremos
                    var cert = releaseInfo.ReleaseDates
                                .FirstOrDefault(r => !string.IsNullOrEmpty(r.Certification))?
                                .Certification;

                    if (!string.IsNullOrEmpty(cert))
                    {
                        clasificacion = cert;
                    }
                }
            }

            // 4. Creación del Objeto
            Pelicula nuevaPelicula = new Pelicula
            {
                Titulo = detalle.Titulo, // Asegúrate que en DetallePelicula mapeaste "title" a Titulo
                Sinopsis = detalle.Sinopsis ?? "Sin descripción disponible.",
                DuracionMinutos = detalle.DuracionMinutos ?? 0,
                PosterUrl = "https://image.tmdb.org/t/p/w500" + detalle.PosterPath,
                TmdbId = detalle.Id,
                EstaEnCartelera = true,

                // Mapeo seguro de Géneros (toma todos separados por coma, es mejor que solo el primero)
                Genero = detalle.Generos != null && detalle.Generos.Any()
                         ? string.Join(", ", detalle.Generos.Select(g => g.Nombre))
                         : "General",

                // Director (Tu lógica estaba bien, solo la protegemos un poco más)
                Director = detalle.Creditos?.Equipo?.FirstOrDefault(p => p.Trabajo == "Director")?.Nombre ?? "Desconocido",

                // Asignamos la clasificación obtenida
                Clasificacion = clasificacion
            };

            try
            {
                _db.Peliculas.Add(nuevaPelicula);
                await _db.SaveChangesAsync();
                return $"Película '{nuevaPelicula.Titulo}' guardada correctamente.";
            }
            catch (Exception ex)
            {
                // Tip: Loguear 'ex.InnerException' suele dar más detalles en EF Core
                return $"Error al guardar BD: {ex.Message} {ex.InnerException?.Message}";
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