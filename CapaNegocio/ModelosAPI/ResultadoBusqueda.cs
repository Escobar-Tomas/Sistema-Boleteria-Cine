using Newtonsoft.Json;
using System.Collections.Generic;

namespace CapaNegocio.ModelosAPI
{
    // Esta clase representa la respuesta global de la búsqueda
    public class ResultadoBusqueda
    {
        [JsonProperty("results")]
        public List<PeliculaBusqueda> Resultados { get; set; }
    }

    // Esta clase representa cada película individual en la lista de búsqueda
    public class PeliculaBusqueda
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Titulo { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("release_date")]
        public string FechaLanzamiento { get; set; }

        // Propiedad calculada para mostrar la imagen en WPF
        public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/150x225?text=Sin+Imagen"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}