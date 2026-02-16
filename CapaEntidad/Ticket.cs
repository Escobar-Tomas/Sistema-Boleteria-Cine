using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapaEntidad
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        // RELACIONES
        public int IdFuncion { get; set; }
        [ForeignKey("IdFuncion")]
        public virtual Funcion Funcion { get; set; }

        // Vendedor (Usuario del sistema)
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        [StringLength(5)]
        public string Asiento { get; set; }

        // Para saber si pagó con Efectivo, Tarjeta, etc.
        [StringLength(50)]
        public string MetodoPago { get; set; }

        // Precio histórico
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;
    }
}