using Newtonsoft.Json;
using System.Collections.Generic;

namespace CapaNegocio.ModelosAPI
{
    public class Creditos
    {
        [JsonProperty("crew")]
        public List<PersonaCrew> Equipo { get; set; }
    }

    public class PersonaCrew
    {
        [JsonProperty("job")]
        public string Trabajo { get; set; } // Buscaremos "Director"

        [JsonProperty("name")]
        public string Nombre { get; set; }
    }
}