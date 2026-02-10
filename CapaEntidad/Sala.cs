using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CapaEntidad
{
    public class Sala
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } // Ej: "Sala 1", "Sala IMAX"

        [Required]
        public int Capacidad { get; set; } // Ej: 50, 100 butacas

        public bool Estado { get; set; } = true;

        // Relación: Una sala tiene muchas funciones
        public virtual ICollection<Funcion> Funciones { get; set; }
    }
}