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

        // Número de asiento (Ej: 1, 2, 3... o "A1")
        [StringLength(5)]
        public string Asiento { get; set; }

        public DateTime FechaVenta { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioPagado { get; set; }

        // Relación
        [ForeignKey("IdFuncion")]
        public virtual Funcion Funcion { get; set; }
    }
}