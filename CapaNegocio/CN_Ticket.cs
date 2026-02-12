using System;
using System.Linq;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio
{
    public class CN_Ticket : ICN_Ticket
    {
        private readonly BDContexto _db;

        // Inyectamos el contexto de base de datos
        public CN_Ticket(BDContexto db)
        {
            _db = db;
        }

        public string RegistrarVenta(Ticket obj)
        {
            try
            {
                // 1. Obtener la función incluyendo los datos de la sala para saber su capacidad
                var funcion = _db.Funciones
                    .Include(f => f.Sala)
                    .FirstOrDefault(f => f.Id == obj.IdFuncion);

                if (funcion == null)
                    return "Error: La función seleccionada no existe.";

                // 2. Validar que la función no haya comenzado
                if (DateTime.Now >= funcion.FechaHoraInicio)
                    return "Error: No se pueden vender tickets para una función que ya ha comenzado.";

                // 3. Validar el Precio (Evita manipulación de datos desde la UI)
                decimal montoCalculado = obj.CantidadTickets * funcion.PrecioTicket;
                if (obj.MontoTotal != montoCalculado)
                    return $"Error de integridad: El monto total no es válido. Debería ser {montoCalculado:C}.";

                // 4. Validar Disponibilidad de Butacas
                // Sumamos los tickets ya vendidos para esta función específica
                int ticketsYaVendidos = _db.Tickets
                    .Where(t => t.IdFuncion == obj.IdFuncion)
                    .Sum(t => (int?)t.CantidadTickets) ?? 0;

                int butacasDisponibles = funcion.Sala.Capacidad - ticketsYaVendidos;

                if (obj.CantidadTickets > butacasDisponibles)
                    return $"Venta rechazada: Solo quedan {butacasDisponibles} butacas disponibles para esta función.";

                // 5. Si pasa todas las validaciones, guardamos la venta
                _db.Tickets.Add(obj);
                _db.SaveChanges();

                return "Venta registrada con éxito.";
            }
            catch (Exception ex)
            {
                return "Error crítico al registrar la venta: " + ex.Message;
            }
        }

        public string AnularVenta(int idTicket)
        {
            try
            {
                var ticket = _db.Tickets.Find(idTicket);
                if (ticket == null)
                    return "El ticket no existe.";

                // Validar que la función no haya pasado (opcional, dependiendo de las reglas del cine)
                var funcion = _db.Funciones.Find(ticket.IdFuncion);
                if (funcion != null && DateTime.Now > funcion.FechaHoraInicio)
                    return "No se puede anular un ticket de una función que ya comenzó o terminó.";

                _db.Tickets.Remove(ticket);
                _db.SaveChanges();

                return "Venta anulada correctamente. Las butacas han sido liberadas.";
            }
            catch (Exception ex)
            {
                return "Error al anular la venta: " + ex.Message;
            }
        }
    }
}