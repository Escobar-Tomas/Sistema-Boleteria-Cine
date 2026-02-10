using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaEntidad
{
    public class Funcion
    {
        [Key]
        public int Id { get; set; }

        // Relación con Película
        public int IdPelicula { get; set; }
        [ForeignKey("IdPelicula")]
        public virtual Pelicula Pelicula { get; set; }

        // Relación con Sala
        public int IdSala { get; set; }
        [ForeignKey("IdSala")]
        public virtual Sala Sala { get; set; }

        [Required]
        public DateTime FechaHoraInicio { get; set; }

        // Calcularemos la hora fin sumando la duración de la película (útil para validar choques)
        public DateTime FechaHoraFin { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTicket { get; set; }

        public bool Estado { get; set; } = true;
    }
}