using CapaDatos;
using CapaEntidad;
using CapaNegocio.Interfaces;
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

        // Implementación de RegistrarVenta
        public bool RegistrarVenta(int idFuncion, int idUsuario, List<string> listaAsientos, decimal precioUnitario, string metodoPago, out string mensaje)
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
                    // Obtener TODOS los ocupados de esta función
                    var ocupadosDb = _db.Tickets
                                        .Where(t => t.IdFuncion == idFuncion)
                                        .Select(t => t.Asiento)
                                        .ToHashSet();

                    foreach (string asiento in listaAsientos)
                    {
                        // Verificamos si ya está ocupado
                        if (ocupadosDb.Contains(asiento))
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
                            Precio = precioUnitario,
                            FechaVenta = DateTime.Now,
                            MetodoPago = metodoPago
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
                    mensaje = "Error al registrar la venta " + ex.Message;
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