using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaEntidad
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public int IdFuncion { get; set; }
        [ForeignKey("IdFuncion")]
        public virtual Funcion Funcion { get; set; }

        // Guarda el código exacto del asiento (Ej: "A1", "B5")
        [Required]
        [StringLength(5)]
        public string Asiento { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;
    }
}