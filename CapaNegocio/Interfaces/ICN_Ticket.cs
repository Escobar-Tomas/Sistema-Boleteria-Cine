using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Ticket
    {
        bool RegistrarVenta(int idFuncion, List<string> listaAsientos, decimal precioUnitario, out string mensaje);

        List<string> ObtenerAsientosOcupados(int idFuncion);

        bool AnularVenta(int idTicket);
    }
}