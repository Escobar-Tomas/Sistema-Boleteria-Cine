using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.ModelosAPI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using CapaNegocio;

namespace CapaNegocio
{
    public class CN_Pelicula
    {
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private readonly BDContexto _db;

        // CAMBIO: Ahora recibimos también la 'cadenaConexion'
        public CN_Pelicula(string apiKey, string cadenaConexion)
        {
            _apiKey = apiKey;

            // Configuramos el contexto con la cadena que nos pasan
            var optionsBuilder = new DbContextOptionsBuilder<BDContexto>();
            optionsBuilder.UseSqlServer(cadenaConexion);

            _db = new BDContexto(optionsBuilder.Options);
        }

        // 1. BUSCAR PELÍCULAS (Para el buscador del WPF)
        public async Task<List<PeliculaBusqueda>> BuscarPeliculasEnTMDB(string nombre)
        {
            using (var client = new HttpClient())
            {
                // Usamos la variable _apiKey
                string url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={nombre}&language=es-ES";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ResultadoBusqueda>(json);
                    return data.Resultados;
                }
            }
            return new List<PeliculaBusqueda>();
        }

        // 2. OBTENER DETALLE Y GUARDAR (Cuando el usuario elige una)
        public async Task<string> GuardarPeliculaDesdeApi(int idTmdb)
        {
            // A. Verificar si ya existe en nuestra BD
            if (await _db.Peliculas.AnyAsync(p => p.TmdbId == idTmdb))
            {
                return "La película ya existe en la base de datos.";
            }

            // B. Descargar detalles completos desde la API
            DetallePelicula detalle = null;
            using (var client = new HttpClient())
            {
                // Pedimos movie details y agregamos 'credits' para sacar el director
                string url = $"{BaseUrl}/movie/{idTmdb}?api_key={_apiKey}&language=es-ES&append_to_response=credits";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    detalle = JsonConvert.DeserializeObject<DetallePelicula>(json);
                }
                else
                {
                    return "Error al conectar con TMDB para obtener detalles.";
                }
            }

            if (detalle == null) return "No se encontraron detalles.";

            // C. Mapear API -> Entidad Base de Datos
            Pelicula nuevaPelicula = new Pelicula
            {
                Titulo = detalle.Titulo,
                Sinopsis = detalle.Sinopsis,
                DuracionMinutos = detalle.DuracionMinutos,
                PosterUrl = "https://image.tmdb.org/t/p/w500" + detalle.PosterPath,
                TmdbId = detalle.Id,
                EstaEnCartelera = true, // Por defecto la ponemos disponible

                // Lógica para obtener géneros (Tomamos el primero o concatenamos)
                Genero = detalle.Generos != null && detalle.Generos.Count > 0
                         ? detalle.Generos[0].Nombre
                         : "General",

                // Lógica para buscar el director
                Director = detalle.Creditos?.Equipo?.FirstOrDefault(p => p.Trabajo == "Director")?.Nombre ?? "Desconocido"
            };

            // D. Guardar en SQL Server
            try
            {
                _db.Peliculas.Add(nuevaPelicula);
                await _db.SaveChangesAsync();
                return $"Película '{nuevaPelicula.Titulo}' guardada correctamente.";
            }
            catch (Exception ex)
            {
                return $"Error al guardar en BD: {ex.Message}";
            }
        }

        // 3. LISTAR LOCALES (Para mostrar en la grilla de mantenimiento)
        public List<Pelicula> ObtenerPeliculasLocales()
        {
            return _db.Peliculas.ToList();
        }

        public List<Pelicula> Listar()
        {
            try
            {
                // Retornamos las películas que ya han sido guardadas en nuestra BD
                return _db.Peliculas.OrderBy(p => p.Titulo).ToList();
            }
            catch (System.Exception)
            {
                return new List<Pelicula>();
            }
        }
    }
}