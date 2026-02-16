using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        public string Sinopsis { get; set; }

        public string Director { get; set; }

        public string Genero { get; set; }

        public int DuracionMinutos { get; set; }

        // Ej: "ATP", "+13", "+16"
        [StringLength(10)]
        public string Clasificacion { get; set; }

        public string PosterUrl { get; set; }

        public int TmdbId { get; set; }

        public bool EstaEnCartelera { get; set; } = true;

        public virtual ICollection<Funcion> Funciones { get; set; }
    }
}