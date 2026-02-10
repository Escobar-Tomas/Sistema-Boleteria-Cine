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

        [JsonProperty("runtime")]
        public int DuracionMinutos { get; set; } // ¡Importante para tu BD!

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("genres")]
        public List<GeneroAPI> Generos { get; set; }

        [JsonProperty("credits")]
        public Creditos Creditos { get; set; }
    }

    public class GeneroAPI
    {
        [JsonProperty("name")]
        public string Nombre { get; set; }
    }
}