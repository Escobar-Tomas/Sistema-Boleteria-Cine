using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaEntidad
{
    public class Funcion
    {
        [Key]
        public int Id { get; set; }

        // RELACIONES
        public int IdPelicula { get; set; }
        [ForeignKey("IdPelicula")]
        public virtual Pelicula Pelicula { get; set; }

        public int IdSala { get; set; }
        [ForeignKey("IdSala")]
        public virtual Sala Sala { get; set; }

        // DATOS DE HORARIO
        [Required]
        public DateTime FechaHoraInicio { get; set; }

        // Se calcula: Inicio + Duración Película + Tiempo Limpieza (opcional)
        public DateTime FechaHoraFin { get; set; }

        // DATOS DE LA PROYECCIÓN 

        [Required]
        [StringLength(10)]
        public string Formato { get; set; } // Ej: "2D", "3D", "IMAX"

        [Required]
        [StringLength(20)]
        public string Idioma { get; set; } // Ej: "Doblada", "Subtitulada", "Original"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTicket { get; set; }

        public bool Estado { get; set; } = true;
    }
}