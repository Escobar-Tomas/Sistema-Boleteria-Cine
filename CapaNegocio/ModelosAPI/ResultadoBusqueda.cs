using Newtonsoft.Json;
using System.Collections.Generic;

namespace CapaNegocio.ModelosAPI
{
    public class ResultadoBusqueda
    {
        [JsonProperty("results")]
        public List<PeliculaBusqueda> Resultados { get; set; }
    }

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

        // Propiedad auxiliar para mostrar la imagen completa en WPF
        public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/150"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}