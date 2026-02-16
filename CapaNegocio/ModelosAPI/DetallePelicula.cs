using Newtonsoft.Json;
using System.Collections.Generic;

namespace CapaNegocio.ModelosAPI
{
    public class DetallePelicula
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Titulo { get; set; }

        [JsonProperty("overview")]
        public string Sinopsis { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("genres")]
        public List<Genero> Generos { get; set; }

        [JsonProperty("runtime")]
        public int? DuracionMinutos { get; set; }

        // Propiedad para el Director y Equipo
        [JsonProperty("credits")]
        public Creditos Creditos { get; set; }

        // Propiedad crítica para la Clasificación (Edad)
        [JsonProperty("release_dates")]
        public ReleaseDatesResult ReleaseDates { get; set; }
    }

    public class Genero
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Nombre { get; set; }
    }

    // --- Estructuras para obtener la certificación (Clasificación) ---

    public class ReleaseDatesResult
    {
        [JsonProperty("results")]
        public List<CountryRelease> Results { get; set; }
    }

    public class CountryRelease
    {
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

        [JsonProperty("release_dates")]
        public List<ReleaseCertification> ReleaseDates { get; set; }
    }

    public class ReleaseCertification
    {
        [JsonProperty("certification")]
        public string Certification { get; set; }
    }
}