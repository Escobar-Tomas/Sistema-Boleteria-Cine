using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces; // Asegúrate de tener este using
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaNegocio
{
    public class CN_Ticket : ICN_Ticket
    {
        private readonly BDContexto _db;

        public CN_Ticket(BDContexto db)
        {
            _db = db;
        }

        public List<string> ObtenerAsientosOcupados(int idFuncion)
        {
            return _db.Tickets
                      .Where(t => t.IdFuncion == idFuncion)
                      .Select(t => t.Asiento)
                      .ToList();
        }

        // Implementación de RegistrarVenta (Coincide con la Interfaz)
        public bool RegistrarVenta(int idFuncion, int idUsuario, List<string> listaAsientos, decimal precioUnitario, out string mensaje)
        {
            mensaje = string.Empty;

            if (listaAsientos == null || !listaAsientos.Any())
            {
                mensaje = "Debe seleccionar al menos un asiento.";
                return false;
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (string asiento in listaAsientos)
                    {
                        // Verificamos si ya está ocupado
                        bool ocupado = _db.Tickets.Any(t => t.IdFuncion == idFuncion && t.Asiento == asiento);
                        if (ocupado)
                        {
                            transaction.Rollback();
                            mensaje = $"El asiento {asiento} ya no está disponible.";
                            return false;
                        }

                        // Creamos el ticket individual
                        Ticket nuevoTicket = new Ticket
                        {
                            IdFuncion = idFuncion,
                            IdUsuario = idUsuario,
                            Asiento = asiento,
                            Precio = precioUnitario, // Precio individual
                            FechaVenta = DateTime.Now
                        };

                        _db.Tickets.Add(nuevoTicket);
                    }

                    _db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    mensaje = "Error al procesar la venta: " + ex.Message;
                    return false;
                }
            }
        }

        // Implementación de AnularVenta (El método que faltaba)
        public bool AnularVenta(int idTicket)
        {
            try
            {
                Ticket ticketEncontrado = _db.Tickets.Find(idTicket);

                if (ticketEncontrado != null)
                {
                    _db.Tickets.Remove(ticketEncontrado);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}