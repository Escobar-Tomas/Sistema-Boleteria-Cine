using CapaEntidad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Sala
    {
        Task<List<Sala>> ListarAsync();
        Task<(bool Exito, string Mensaje)> CrearAsync(Sala sala);
        Task<(bool Exito, string Mensaje)> EditarAsync(Sala sala);
        Task<bool> EliminarAsync(int id);
    }
}