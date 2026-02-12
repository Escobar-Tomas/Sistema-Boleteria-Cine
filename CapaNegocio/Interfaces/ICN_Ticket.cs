using CapaEntidad;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Ticket
    {
        string RegistrarVenta(Ticket obj);
        string AnularVenta(int idTicket);
    }
}