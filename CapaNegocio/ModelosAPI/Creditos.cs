using Newtonsoft.Json;
using System.Collections.Generic;

namespace CapaNegocio.ModelosAPI
{
    public class Creditos
    {
        // TMDB devuelve "crew". Lo mapeamos a "Equipo" para tu lógica.
        [JsonProperty("crew")]
        public List<MiembroEquipo> Equipo { get; set; }

        [JsonProperty("cast")]
        public List<MiembroReparto> Reparto { get; set; }
    }

    public class MiembroEquipo
    {
        [JsonProperty("name")]
        public string Nombre { get; set; }

        // TMDB devuelve "job". Lo mapeamos a "Trabajo".
        [JsonProperty("job")]
        public string Trabajo { get; set; }
    }

    public class MiembroReparto
    {
        [JsonProperty("name")]
        public string Nombre { get; set; }

        [JsonProperty("character")]
        public string Personaje { get; set; }
    }
}