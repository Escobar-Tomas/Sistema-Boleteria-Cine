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

        // CAMBIADO: De 'Descripcion' a 'Sinopsis' para coincidir con tu error
        public string Sinopsis { get; set; }

        public string Director { get; set; }
        public string Genero { get; set; }
        public int DuracionMinutos { get; set; }

        // CAMBIADO: De 'ImagenPortada' a 'PosterUrl'
        public string PosterUrl { get; set; }

        // CAMBIADO: De 'IdTMDB' a 'TmdbId' (Notación PascalCase estándar)
        public int TmdbId { get; set; }

        // CAMBIADO: Agregamos esta propiedad que te falta en el error
        public bool EstaEnCartelera { get; set; } = true;

        public virtual ICollection<Funcion> Funciones { get; set; }
    }
}