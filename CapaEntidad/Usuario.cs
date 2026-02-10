using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        public string Clave { get; set; } // Aquí guardaremos el HASH, no la clave real

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } // "Administrador" o "Empleado"

        public bool Estado { get; set; } = true;
    }
}