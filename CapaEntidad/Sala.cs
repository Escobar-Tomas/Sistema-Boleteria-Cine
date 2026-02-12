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
        public string Nombre { get; set; }

        [Required]
        public int Capacidad { get; set; }

        public bool Estado { get; set; } = true;

        [Required]
        public int Filas { get; set; }

        [Required]
        public int Columnas { get; set; }

        // Relación: Una sala tiene muchas funciones
        public virtual ICollection<Funcion> Funciones { get; set; }
    }
}