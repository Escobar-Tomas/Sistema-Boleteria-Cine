using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CapaNegocio.ModelosAPI;
using Newtonsoft.Json;

namespace CapaNegocio.Servicios
{
    public class TmdbService : ITmdbService
    {
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.themoviedb.org/3";

        // CAMBIO 1: Declaramos el HttpClient como 'static' y 'readonly'.
        // 'static' significa que esta instancia es compartida por toda la aplicación.
        // Se crea una vez y se reutiliza siempre.
        private static readonly HttpClient _httpClient = new HttpClient();

        public TmdbService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<List<PeliculaBusqueda>> BuscarPeliculasAsync(string nombre)
        {
            // CAMBIO 2: Quitamos el bloque 'using (var client = ...)'
            // Usamos directamente _httpClient
            string url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={nombre}&language=es-ES";

            // Usamos ConfigureAwait(false) para mejorar el rendimiento interno
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<ResultadoBusqueda>(json);
                // Validación extra por si la API devuelve nulo
                return data?.Resultados ?? new List<PeliculaBusqueda>();
            }

            return new List<PeliculaBusqueda>();
        }

        public async Task<DetallePelicula> ObtenerDetallePeliculaAsync(int idTmdb)
        {
            // CAMBIO 3: Igual aquí, usamos la instancia compartida
            string url = $"{BaseUrl}/movie/{idTmdb}?api_key={_apiKey}&language=es-ES&append_to_response=credits";

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<DetallePelicula>(json);
            }

            return null;
        }
    }
}