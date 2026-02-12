using System.Collections.Generic;
using CapaEntidad;

namespace CapaNegocio.Interfaces
{
    public interface ICN_Sala
    {
        List<Sala> Listar();
        string Guardar(Sala obj);
        void Eliminar(int id);
    }
}