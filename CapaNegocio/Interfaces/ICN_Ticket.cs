using CapaEntidad;
using System.Collections.Generic;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Ticket
    {
        bool RegistrarVenta(int idFuncion, int idUsuario, List<string> listaAsientos, decimal precioUnitario, string metodoPago, out string mensaje);

        List<string> ObtenerAsientosOcupados(int idFuncion);

        bool AnularVenta(int idTicket);
    }
}